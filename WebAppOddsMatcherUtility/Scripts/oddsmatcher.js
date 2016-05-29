// Declare namespace
var ODDS_NS = ODDS_NS || {};

ODDS_NS.g_stake = parseFloat(10.00);
ODDS_NS.g_betType = null;

ODDS_NS.init = function()
{
    $('[data-toggle="popover"]').popover();
    // filter check boxes initally all set to ticked ONLY if there is no query string
    //if (window.location.search.length > 0)
    //{
        //TODO: populate the filters
    //    console.log("We got filters to apply to the UI");
    //}
    //else
    //{
        // first time onto the page so no filters have been applied, check em all
    //    $('#cb_sport_all').prop('checked', true);
    //    $('.cb_sport').prop('checked', true);
    //    $('#cb_market_all').prop('checked', true);
    //    $('.cb_market').prop('checked', true);
    //    $('#cb_bookmaker_all').prop('checked', true);
    //    $('.cb_bookmaker').prop('checked', true);
    //    $('#cb_exchange_all').prop('checked', true);
    //    $('.cb_exchange').prop('checked', true);
    //}
    

    $("#timeframe").ionRangeSlider({
        grid: true,
        from: 0,
        force_edges: true,
        values: [
            "Now", "30mins", "1hr", "12hrs", "1day", "3days", "7days", "No limit"
        ]
    });

    // click / change handlers for setting the 'all' filter tick boxes
    $('#cb_sport_all').click(function () {
        $('.cb_sport').prop('checked', this.checked);
    });
    $('#cb_market_all').click(function () {
        $('.cb_market').prop('checked', this.checked);
    });
    $('#cb_bookmaker_all').click(function () {
        $('.cb_bookmaker').prop('checked', this.checked);
    });
    // for now disable this as we only have one exchange
    //$('#cb_exchange_all').click(function () {
    //    $('.cb_exchange').prop('checked', this.checked);
    //});

    $('.cb_sport').change(function () {
        var check = ($('.cb_sport').filter(":checked").length == $('.cb_sport').length);
        $('#cb_sport_all').prop('checked', check);
    });
    $('.cb_market').change(function () {
        var check = ($('.cb_market').filter(":checked").length == $('.cb_market').length);
        $('#cb_market_all').prop('checked', check);
    });
    $('.cb_bookmaker').change(function () {
        var check = ($('.cb_bookmaker').filter(":checked").length == $('.cb_bookmaker').length);
        $('#cb_bookmaker_all').prop('checked', check);
    });
    // for now disable this as we only have one exchange
    //$('.cb_exchange').change(function () {
    //    var check = ($('.cb_exchange').filter(":checked").length == $('.cb_exchange').length);
    //    $('#cb_exchange_all').prop('checked', check);
    //});

    // change handler for the bet type radio buttons
    $('input[name="bet_type_selector"]').on('change', ODDS_NS.inlineCalculate);

    // call inline calculator with preset values
    ODDS_NS.inlineCalculate();

    //
    // Modal caclulator functions
    //
    $('#calculatorModal').on('shown.bs.modal', ODDS_NS.calculatorModalInit);
    $('#calculatorModal').on('hidden.bs.modal', ODDS_NS.clearModalCalculatorData);
    $('#calculatorModal').on('change', ODDS_NS.modalCalculate);
}

// TODO: deprecate this method
ODDS_NS.submitOldFilterForm = function(selectObj) {
    selectObj.form.submit();
}

ODDS_NS.submitFilterForm = function (applyBtn) {
    //TODO: may need some logic in here before sending form to server
    applyBtn.form.submit();
}

//ODDS_NS.submitClearFilterForm = function (clearBtn) {
//    //TODO: may need some logic in here before sending form to server
//    document.getElementById("search_filter").reset();
//}

ODDS_NS.refreshPage = function() {
    location.reload();
}

ODDS_NS.inlineCalculate = function(event)
{
    // get the stake value, default it to 10.00 if the field is cleared
    var stake_val = document.getElementById('stake_amount_val').value;
    // if you type a character for now default it to £10.00 - TODO: this will need to change!
    if (isNaN(stake_val)) {
        document.getElementById('stake_amount_val').value = '10.00';
        stake_val = '10.00';
    }

    // set the global variable, default to £10.00 if the field is empty
    ODDS_NS.g_stake = stake_val.length === 0 ? parseFloat(10.00) : parseFloat(stake_val);
    
    ODDS_NS.g_betType = document.querySelector('input[name="bet_type_selector"]:checked').value;
    $('#odds_table > tbody > tr').each( ODDS_NS.calc );
        
}

ODDS_NS.calc = function( i, row )
{
    $(row).find('span#stake_entered').html( "£" + parseFloat(ODDS_NS.g_stake).toFixed(2) );

    var dataButton = $(row).find('td:last > button');
    var scriptBackOdds = dataButton.data('back-odds');
    var scriptBackComm = dataButton.data('back-comm');
    var scriptLayOdds = dataButton.data('lay-odds');
    var scriptLayComm = dataButton.data('lay-comm');

    //
    // Calculate Qualifier
    //
    
    if (ODDS_NS.g_betType == "optionQualifier") {
        // Calculate Lay bet
        // E3/(E4-F4)*D3
        var resultLayBet = scriptBackOdds / (scriptLayOdds - scriptLayComm) * ODDS_NS.g_stake;
        $(row).find('span#you_lay').html("£" + resultLayBet.toFixed(2));

        // Calculate Liability
        // D4*(E4-1))
        var resultLiability = resultLayBet * (scriptLayOdds - 1);
        $(row).find('#liability').html("£" + resultLiability.toFixed(2));

        //
        // Bookmaker Win
        //
        // Bookie Balance
        // D3 * (E3 - 1) * (1 - F3)
        var resultBookmakerWinBB = ODDS_NS.g_stake * (scriptBackOdds - 1) * (1 - scriptBackComm)
        // Exchange Balance
        var resultBookmakerWinXB = resultLiability * -1
        // Profit
        // K3-G3
        var resultBookmakerWinProfit = resultBookmakerWinBB + resultBookmakerWinXB
        $(row).find('span#bookmaker_position').html("£" + resultBookmakerWinProfit.toFixed(2) );


        //
        // Exchange Win
        //
        // Exchange Balance
        // -1 * D3
        var resultExchangeWinBB = -1 * ODDS_NS.g_stake;
        // Exchange Balance
        // D4*(1-F4)
        var resultExchangeWinXB = resultLayBet * (1 - scriptLayComm)
        // Profit
        // K4-D3
        var resultExchangeWinProfit = resultExchangeWinXB + resultExchangeWinBB
        $(row).find('span#exchange_position').html("£" +  resultExchangeWinProfit.toFixed(2) );

    }

    //
    // Calculate optionSNR
    //
    if (ODDS_NS.g_betType == "optionSNR") {

        // Calculate Lay bet
        //(E3-1)/(E4-F4)*D3
        var resultLayBet = (scriptBackOdds - 1) / (scriptLayOdds - scriptLayComm) * ODDS_NS.g_stake;
        $(row).find('span#you_lay').html("£" + resultLayBet.toFixed(2));

        // Calculate Liability
        // D4*(E4-1))
        var resultLiability = resultLayBet * (scriptLayOdds - 1);
        $(row).find('#liability').html("£" + resultLiability.toFixed(2));

        //
        // Bookmaker Win
        //
        // Bookie Balance
        // D3 * (E3 - 1) * (1 - F3)
        var resultBookmakerWinBB = ODDS_NS.g_stake * (scriptBackOdds - 1) * (1 - scriptBackComm)
        // Exchange Balance
        var resultBookmakerWinXB = resultLiability * -1
        // Profit
        var resultBookmakerWinProfit = resultBookmakerWinBB + resultBookmakerWinXB
        $(row).find('span#bookmaker_position').html("£" + resultBookmakerWinProfit.toFixed(2));

        //
        // Exchange Win
        //
        // Exchange Balance
        var resultExchangeWinBB = 0
        // Exchange Balance
        // D4*(1-F4)
        var resultExchangeWinXB = resultLayBet * (1 - scriptLayComm)
        // Profit
        var resultExchangeWinProfit = resultExchangeWinXB
        $(row).find('span#exchange_position').html("£" + resultExchangeWinProfit.toFixed(2));

    }

    //
    // Calculate optionSR
    //
    if (ODDS_NS.g_betType == "optionSR") {

        // Calculate Lay bet
        // E3/(E4-F4)*D3
        var resultLayBet = scriptBackOdds / (scriptLayOdds - scriptLayComm) * ODDS_NS.g_stake;
        $(row).find('span#you_lay').html("£" + resultLayBet.toFixed(2));

        // Calculate Liability
        // D4*(E4-1))
        var resultLiability = resultLayBet * (scriptLayOdds - 1);
        $(row).find('#liability').html("£" + resultLiability.toFixed(2));

        //
        // Bookmaker Win
        //
        // Bookie Balance
        // D3*E3*(1-F3)
        var resultBookmakerWinBB = ODDS_NS.g_stake * scriptBackOdds * (1 - scriptBackComm)
        // Exchange Balance
        var resultBookmakerWinXB = resultLiability * -1
        // Profit
        // K3+D3+K4
        var resultBookmakerWinProfit = resultBookmakerWinBB + resultBookmakerWinXB + (1 * ODDS_NS.g_stake)
        $(row).find('span#bookmaker_position').html("£" + resultBookmakerWinProfit.toFixed(2));

        //
        // Exchange Win
        //
        // Exchange Balance
        var resultExchangeWinBB = 0
        // Exchange Balance
        // D4*(1-F4)
        var resultExchangeWinXB = resultLayBet * (1 - scriptLayComm)
        // Profit
        var resultExchangeWinProfit = resultExchangeWinXB
        $(row).find('span#exchange_position').html("£" + resultExchangeWinProfit.toFixed(2));

    }

    // Colour the Bet Profit fields red when not a profit
    if (resultBookmakerWinProfit < 0) 
        $(row).find('span#bookmaker_position').css('color', 'red');
    else
        $(row).find('span#bookmaker_position').css('color', 'green');

    if (resultExchangeWinProfit < 0)
        $(row).find('span#exchange_position').css('color', 'red');
    else
        $(row).find('span#exchange_position').css('color', 'green');
}

//
// Modal caclulator functions
//

ODDS_NS.clearModalCalculatorData = function(e) {
    $(this).data('bs.modal', null);
}

ODDS_NS.calculatorModalInit = function( event ) {
    // Set-up variables from modal screen
    var button = $(event.relatedTarget) // Button that triggered the modal
    var scriptEvent = button.data('event')
    var scriptEventDate = button.data('event-date')
    var scriptStake = button.data('stake')
    var scriptOdds = button.data('back-odds')
    var scriptComm = button.data('back-comm')
    var scriptBookmakerName = button.data('bookmaker-name')
    var scriptLayOdds = button.data('lay-odds')
    var scriptLayComm = button.data('lay-comm')
    var scriptExchangeName = button.data('exchange-name')
    var scriptMarketName = button.data('market-name')
    var scriptBetName = button.data('bet-name')

    var modal = $(this)
    modal.find('.modal-body #event-name').val(scriptEvent)
    modal.find('.modal-body #textareaID').val(scriptEvent)
    modal.find('.modal-body #event-date').val(scriptEventDate)
    modal.find('.modal-body #stake').val(scriptStake)
    modal.find('.modal-body #back-odds').val(scriptOdds)
    modal.find('.modal-body #back-comm').val(scriptComm)
    modal.find('.modal-body #bookmaker-name').val(scriptBookmakerName)
    modal.find('.modal-body #lay-odds').val(scriptLayOdds)
    modal.find('.modal-body #lay-comm').val(scriptLayComm)
    modal.find('.modal-body #exchange-name').val(scriptExchangeName)
    modal.find('.modal-body #market-name').val(scriptMarketName)
    modal.find('.modal-body #bet-name').val(scriptBetName)

    // Set blank values for calculated fields
    modal.find('.modal-body #lay-amount').val('')
    modal.find('.modal-body #liability-amount').val('')
    modal.find('.modal-body #bookie-win-bbal').val('')
    modal.find('.modal-body #bookie-win-xbal').val('')
    modal.find('.modal-body #bookie-win-profit').val('')
    modal.find('.modal-body #exchange-win-bbal').val('')
    modal.find('.modal-body #exchange-win-xbal').val('')
    modal.find('.modal-body #exchange-win-profit').val('')


    // Select "Qualifier" radio button
    $('#inlineRadioQualifier').prop('checked', true);

    // Set focus to
    $('#stake').focus();

}

ODDS_NS.modalCalculate = function( event )
{
    var objectTrigger = $(event.relatedTarget) // "event" (not Button) that triggered the modal

    var radio = jQuery('input[name="inlineRadioOptions"]:checked');
    var radioButtonvalue = radio.val();

    var scriptStake = document.getElementById("stake").value;
    var scriptBackOdds = document.getElementById("back-odds").value;
    var scriptBackComm = document.getElementById("back-comm").value;
    var scriptLayOdds = document.getElementById("lay-odds").value;
    var scriptLayComm = document.getElementById("lay-comm").value;

    //
    // Calculate Qualifier
    //
    if (radioButtonvalue == "optionQualifier") {

        // Calculate Lay bet
        // E3/(E4-F4)*D3
        var resultLayBet = scriptBackOdds / (scriptLayOdds - scriptLayComm) * scriptStake;
        document.getElementById("lay-amount").value = resultLayBet.toFixed(2);

        // Calculate Liability
        // D4*(E4-1))
        var resultLiability = resultLayBet * (scriptLayOdds - 1);
        document.getElementById("liability-amount").value = resultLiability.toFixed(2);

        //
        // Bookmaker Win
        //
        // Bookie Balance
        // D3 * (E3 - 1) * (1 - F3)
        var resultBookmakerWinBB = scriptStake * (scriptBackOdds - 1) * (1 - scriptBackComm)
        document.getElementById("bookie-win-bbal").value = resultBookmakerWinBB.toFixed(2);

        // Exchange Balance
        var resultBookmakerWinXB = resultLiability * -1
        document.getElementById("bookie-win-xbal").value = resultBookmakerWinXB.toFixed(2);

        // Profit
        // K3-G3
        var resultBookmakerWinProfit = resultBookmakerWinBB + resultBookmakerWinXB
        document.getElementById("bookie-win-profit").value = resultBookmakerWinProfit.toFixed(2);

        //
        // Exchange Win
        //
        // Exchange Balance
        // -1 * D3
        var resultExchangeWinBB = -1 * scriptStake;
        document.getElementById("exchange-win-bbal").value = resultExchangeWinBB.toFixed(2);

        // Exchange Balance
        // D4*(1-F4)
        var resultExchangeWinXB = resultLayBet * (1 - scriptLayComm)
        document.getElementById("exchange-win-xbal").value = resultExchangeWinXB.toFixed(2);

        // Profit
        // K4-D3
        var resultExchangeWinProfit = resultExchangeWinXB + resultExchangeWinBB
        document.getElementById("exchange-win-profit").value = resultExchangeWinProfit.toFixed(2);

    }

    //
    // Calculate optionSNR
    //
    if (radioButtonvalue == "optionSNR") {

        // Calculate Lay bet
        //(E3-1)/(E4-F4)*D3
        var resultLayBet = (scriptBackOdds - 1) / (scriptLayOdds - scriptLayComm) * scriptStake;
        document.getElementById("lay-amount").value = resultLayBet.toFixed(2);

        // Calculate Liability
        // D4*(E4-1))
        var resultLiability = resultLayBet * (scriptLayOdds - 1);
        document.getElementById("liability-amount").value = resultLiability.toFixed(2);

        //
        // Bookmaker Win
        //
        // Bookie Balance
        // D3 * (E3 - 1) * (1 - F3)
        var resultBookmakerWinBB = scriptStake * (scriptBackOdds - 1) * (1 - scriptBackComm)
        document.getElementById("bookie-win-bbal").value = resultBookmakerWinBB.toFixed(2);

        // Exchange Balance
        var resultBookmakerWinXB = resultLiability * -1
        document.getElementById("bookie-win-xbal").value = resultBookmakerWinXB.toFixed(2);

        // Profit
        var resultBookmakerWinProfit = resultBookmakerWinBB + resultBookmakerWinXB
        document.getElementById("bookie-win-profit").value = resultBookmakerWinProfit.toFixed(2);

        //
        // Exchange Win
        //
        // Exchange Balance
        var resultExchangeWinBB = 0
        document.getElementById("exchange-win-bbal").value = resultExchangeWinBB.toFixed(2);

        // Exchange Balance
        // D4*(1-F4)
        var resultExchangeWinXB = resultLayBet * (1 - scriptLayComm)
        document.getElementById("exchange-win-xbal").value = resultExchangeWinXB.toFixed(2);

        // Profit
        var resultExchangeWinProfit = resultExchangeWinXB
        document.getElementById("exchange-win-profit").value = resultExchangeWinProfit.toFixed(2);

    }

    //
    // Calculate optionSR
    //
    if (radioButtonvalue == "optionSR") {

        // Calculate Lay bet
        // E3/(E4-F4)*D3
        var resultLayBet = scriptBackOdds / (scriptLayOdds - scriptLayComm) * scriptStake;
        document.getElementById("lay-amount").value = resultLayBet.toFixed(2);

        // Calculate Liability
        // D4*(E4-1))
        var resultLiability = resultLayBet * (scriptLayOdds - 1);
        document.getElementById("liability-amount").value = resultLiability.toFixed(2);

        //
        // Bookmaker Win
        //
        // Bookie Balance
        // D3*E3*(1-F3)
        var resultBookmakerWinBB = scriptStake * scriptBackOdds * (1 - scriptBackComm)
        document.getElementById("bookie-win-bbal").value = resultBookmakerWinBB.toFixed(2);

        // Exchange Balance
        var resultBookmakerWinXB = resultLiability * -1
        document.getElementById("bookie-win-xbal").value = resultBookmakerWinXB.toFixed(2);

        // Profit
        // K3+D3+K4
        var resultBookmakerWinProfit = resultBookmakerWinBB + resultBookmakerWinXB + (1 * scriptStake)
        document.getElementById("bookie-win-profit").value = resultBookmakerWinProfit.toFixed(2);

        //
        // Exchange Win
        //
        // Exchange Balance
        var resultExchangeWinBB = 0
        document.getElementById("exchange-win-bbal").value = resultExchangeWinBB.toFixed(2);

        // Exchange Balance
        // D4*(1-F4)
        var resultExchangeWinXB = resultLayBet * (1 - scriptLayComm)
        document.getElementById("exchange-win-xbal").value = resultExchangeWinXB.toFixed(2);

        // Profit
        var resultExchangeWinProfit = resultExchangeWinXB
        document.getElementById("exchange-win-profit").value = resultExchangeWinProfit.toFixed(2);

    }

    // Colour the Bet Profit fields red when not a profit
    if (resultBookmakerWinProfit < 0) {
        var targetbox = document.getElementById("bookie-win-profit");
        targetbox.style.backgroundColor = "red"
        targetbox.style.color = "white"
    }
    if (resultExchangeWinProfit < 0) {
        var targetbox = document.getElementById("exchange-win-profit");
        targetbox.style.backgroundColor = "red"
        targetbox.style.color = "white"
    }

    // Colour the Bet Profit fields Green when a profit
    if (resultBookmakerWinProfit > 0) {
        var targetbox = document.getElementById("bookie-win-profit");
        targetbox.style.backgroundColor = "green"
        targetbox.style.color = "white"
    }
    if (resultExchangeWinProfit > 0) {
        var targetbox = document.getElementById("exchange-win-profit");
        targetbox.style.backgroundColor = "green"
        targetbox.style.color = "white"
    }

}

