var Match = Match || {};
Match.global = Match.global || {};

Match.onload = function () {
    Match.global.pageLoad();
};

Match.global.pageLoad = function (f) {
    this.queue = this.queue || [];
    // add functions to the queue
    if (arguments.length > 0) {
        var loaded = this.queue.length;
        for (var i = 0; i < arguments.length; i++) {
            if (typeof arguments[i] == "function") {
                this.queue[i + loaded] = arguments[i];
            } else {
                throw ('Global Page Load - Argument must be a function: ' + arguments[i]);
            }
        }
        // run all functions in the queue
    } else if (this.queue.length > 0) {
        for (var i = 0; i < this.queue.length; i++) {
            this.queue[i]();
        }
    } else {
        return false;
    }
};


Match.Aggregate = Match.Aggregate || {};

Match.Aggregate.init = function () {
};

Match.ingredientsMatching = Match.ingredientsMatching || {};

Match.ingredientsMatching.wireRadioButtons = function () {
    $('.match-control input:radio').live("click", function (e) {
        e.preventDefault();
        $('#' + this.name + '-matched').load("/MatchedItems/UpdateIngredientMatch?xmlguid=" + this.name + "&matchedGuid=" + this.value);
        $(this).closest(".dialog").dialog("close");
    });

    $('.menu-match-control input:radio').live("click", function (e) {
        e.preventDefault();
        $('#' + this.name + '-matched').load("/MatchedItems/UpdateMenuItemsMatch?xmlguid=" + this.name + "&matchedGuid=" + this.value);
        $(this).closest(".dialog").dialog("close");
    });
    
};

Match.Aggregate.wireModals = function () {
    $.ajaxSetup({ cache: false });
    $(".openDialog").live("click", function (e) {
        e.preventDefault();


        var dia = $('<div></div>');
        dia.addClass('dialog');
        dia.appendTo('body');
        dia.dialog({
            title: $(this).attr("data-dialog-title"),
            close: function () { $(this).remove(); },
            width: 1001,
                height: 800,
            position: 'top',
            modal: true
        });
        dia.attr('id', $(this).attr('data-dialog-id'));
        dia.load(this.href);

    });



    $(".close").live("click", function (e) {
        e.preventDefault();
        $(this).closest(".dialog").dialog("close");
    });

};
Match.global.pageLoad(Match.Aggregate.init, Match.Aggregate.wireModals, Match.ingredientsMatching.wireRadioButtons);

$(document).ready(function () {
    Match.onload();
});
