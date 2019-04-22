
$(window).on("load", function () {

    var dateField = function(config) {
        jsGrid.Field.call(this, config);
    };

    dateField.prototype = new jsGrid.Field({
        sorter: function(date1, date2) {
            return new Date(date1) - new Date(date2);
        },

        itemTemplate: function(value) {
            return formatDate(new Date(value));
        },

        filterTemplate: function() {
            var now = new Date();
            this._fromPicker = $("<input>").datepicker({ defaultDate: now.setFullYear(now.getFullYear() - 1) });
            this._toPicker = $("<input>").datepicker({ defaultDate: now.setFullYear(now.getFullYear() + 1) });
            return $("<div>").append(this._fromPicker).append(this._toPicker);
        },

        insertTemplate: function(value) {
            return this._insertPicker = $("<input>").datepicker({ defaultDate: new Date() });
        },

        editTemplate: function(value) {
            return this._editPicker =
                $("<input>").datepicker({ dateFormat: "dd.mm.yy" }).datepicker("setDate", new Date(value));
        },

        insertValue: function() {
            //return this._insertPicker.datepicker("getDate").toISOString();
            var insertValue = this._insertPicker.datepicker("getDate");
            if (insertValue !== null && insertValue !== "undefined") {
                return formatDate(this._insertPicker.datepicker("getDate"));
            }
            return null;
        },

        editValue: function() {
            //return this._editPicker.datepicker("getDate").toISOString();
            var editValue = this._editPicker.datepicker("getDate");
            if (editValue !== null && editValue !== "undefined") {
                return formatDate(this._editPicker.datepicker("getDate"));
            }
            return null;
        },

        filterValue: function() {
            return {
                from: this._fromPicker.datepicker("getDate"),
                to: this._toPicker.datepicker("getDate")
            };
        }
    });

    jsGrid.fields.date = dateField;
});

function initiateJsGrid(key, value, baseUrl) {
    var postponeLevels = [
        { Lvl: 0 },
        { Lvl: 1 },
        { Lvl: 2 },
        { Lvl: 6 },
        { Lvl: 15 },
        { Lvl: 40 },
        { Lvl: 100 },
        { Lvl: 250 },
        { Lvl: 625 }
    ];

    $("#jsGrid").jsGrid({
        width: "100%",
        height: "400px",

        //inserting: true,
        editing: true,
        sorting: true,
        paging: true,

        noDataContent: "Not found",
        autoload: true,

        //data: data,
        controller: {
            loadData: function (filter) {
                return asyncGet(baseUrl + "Find/" + key + "/" + value);
            },

            insertItem: function (memo) {
                asyncPost(baseUrl + "Add", memo);
            },

            updateItem: function (memo) {
                asyncPost(baseUrl + "Update", memo);
            },

            deleteItem: function (memo) {
                asyncPost(baseUrl + "Delete/" + memo.id);
            }
        },

        loadIndication: true,
        loadIndicationDelay: 500,
        loadMessage: "Loading memos...",
        loadShading: true,

        updateOnResize: true,

        fields: [
            { name: "id", type: "number", title: "ID", align: "center", editing: false, width: 50 },
            { name: "question", type: "text", title: "Question", align: "center", width: 150 },
            { name: "answer", type: "text", title: "Answer", align: "center", width: 200 },
            { name: "repeatDate", type: "date", formatter: "date", title: "Repeat date", align: "center", width: 90 },
            {
                name: "postponeLevel", type: "select", title: "LVL", align: "center", width: 40,
                items: postponeLevels, valueField: "Lvl", textField: "Lvl"
            },
            { name: "scores", type: "number", title: "Scores", align: "center", width: 65 },
            { type: "control", width: 52 }
        ]
    });
}

function formatDate(date) {
    var d = new Date(date),
        month = "" + (d.getMonth() + 1),
        day = "" + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = "0" + month;
    if (day.length < 2) day = "0" + day;

    return [day, month, year].join(".");
}

$(document).ready(function () {

    window.history.replaceState(null, null, window.location.href);

    if (sessionStorage.hasOwnProperty("alert")) {
        showMessage(sessionStorage.getItem("alert"));
        sessionStorage.removeItem("alert");
    }
});

function initiate(baseUrl, memoModel) {

    $("#showAnswer").focus();

    $("#showAnswer").click(function () {
        $("#answerArea").val(memoModel.answer);
        $("#answerForm").css("visibility", "visible");
        $("input[value=Cool]").focus();
    });

    $("#questionArea,#answerArea").focus(function () {
        $(".update-group").css("visibility", "visible");
    });

    $("#questionArea,#answerArea").blur(function () {
        if ($(".update-group:hover").length === 0) {
            $(".update-group").css("visibility", "hidden");
        }
    });

    $("#replaceMemos").click(function () {
        var question = $("#questionArea").val();
        var answer = $("#answerArea").val();
        $("#questionArea").val(answer);
        $("#answerArea").val(question);
    });

    $("#switchModeButton").click(function (event) {
        window.location = baseUrl.concat(
            $(this).text().includes("Learn") ? "Learn" : "Repeat");

        event.preventDefault();
    });

    $("#updateButton").click(function (event) {
        memoModel.question = $("#questionArea").val();
        memoModel.answer = $("#answerArea").val();
        asyncPost(baseUrl + "Update", memoModel);

        $(".update-group").css("visibility", "hidden");
        event.preventDefault();
    });

    $("#deleteButton").click(function (event) {
        asyncPost(baseUrl + "Delete/" + memoModel.id,
            null,
            function (response) {
                sessionStorage.setItem("alert", response);
                location.reload(true);
            });

        event.preventDefault();
    });

    $("#answerForm").submit(function () {
        $(".lds-ellipsis").css("visibility", "visible");
    });

    initiateModals(baseUrl);
};

function showMessage(response) {
    $("#message").text(response);
    var className = !response.startsWith("Error") ? "alert-success" : "alert-danger";
    $(".alert").addClass(className);

    $(".alert").removeClass("d-none");
}

function asyncPost(url, data, callback) {
    $(".lds-ellipsis").css("visibility", "visible");

    return $.ajax({
        url: url,
        type: "POST",
        data: data,
        complete: function () {
            $(".lds-ellipsis").css("visibility", "hidden");
        },
        success: function (response) {

            if (callback != undefined)
                callback(response);

            showMessage(response);
        }
    });
}

function asyncGet(actionLink, callback) {
    return $.ajax({
        url: actionLink,
        type: "GET",
        success: function (data) {
            if (callback != undefined)
                callback(data);
        }
    });
}

function finish(mode) {
    $(".summary").css("visibility", "hidden");
    $("#finishMessage").text(mode);
    $("#finishModal").modal();
}

function initiateModals(baseUrl) {
    $(window).click(function (event) {
        if ($(".navbar-collapse").hasClass("show") && !$(event.target).hasClass("navbar-toggler")) {
            $("button.navbar-toggler").click();
        }
    });

    $("#addModal").on("hidden.bs.modal", function () {
        $(this).find(".modal-body > textarea").val("");
    });

    $("#findNewMemo").click(function () {
        var key = $("#findKey").val();
        var value = $("#findValue").val();
        initiateJsGrid(key, value, baseUrl);
        $("#findModal").find(".modal-dialog").css("max-width", "90%");
    });

    $("#submitNewMemo").click(function () {
        asyncPost(baseUrl + "Add",
            "question=" + $("#addQuestionArea").val() +
            "&answer=" + $("#addAnswerArea").val());
    });

    $("#findModal").on("hidden.bs.modal", function () {
        $("#findKey").val("");
        if ($("#jsGrid").children().length > 0)
            $("#jsGrid").jsGrid("destroy");
        $(this).find(".modal-dialog").css("max-width", "");
    });
};