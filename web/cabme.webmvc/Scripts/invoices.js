$(document).ready(function () {
    ko.applyBindings(new InvoiceViewModel());
});
function Invoice(total, items, filter) {
    var self = this;
    self.total = total;
    self.items = ko.observableArray();
    self.filter = filter;
    $.each(items, function (index, item) {
        self.items.push({ refCode: item.RefCode, time: item.PickupTime, suburbFrom: item.SuburbFrom });
    });
    this.itemsToShow = ko.computed(function () {
        var filter = this.filter();
        if (filter == '') return this.items();
        return ko.utils.arrayFilter(this.items(), function (item) {
            return item.refCode.toUpperCase().lastIndexOf(filter.toUpperCase(), 0) === 0;
        });
    }, this);
}
function InvoiceViewModel() {
    var self = this;
    self.filter = ko.observable('');
    self.invoice = ko.observableArray();
    self.monthYear = ko.observable('');
    self.detailed = ko.observable(false);
    self.loadData = function (year, month) {
        if (taxiId) {
            $.getJSON('/service/cabmeservice.svc/invoice?name=' + taxiId + '&month=' + month + '&year=' + year, function (invoice) {
                if (invoice != null) {
                    self.invoice.removeAll();
                    var invoice = new Invoice(invoice.Total, invoice.Items, self.filter);
                    self.invoice.push(invoice);
                }
            });
        }
    };
}
ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        //initialize datepicker with some optional options
        var options = allBindingsAccessor().datepickerOptions || {};
        $(element).datepicker(options);

        //handle the field changing
        ko.utils.registerEventHandler(element, "change", function () {
            var observable = valueAccessor();
            observable($(element).datepicker("getDate"));
        });

        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).datepicker("destroy");
        });

    },
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor()),
            current = $(element).datepicker("getDate");

        if (value - current !== 0) {
            $(element).datepicker("setDate", value);
        }
    }
};