
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
