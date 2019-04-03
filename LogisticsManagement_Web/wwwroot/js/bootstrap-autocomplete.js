$.widget("ui.autocomplete", $.ui.autocomplete, {

    _renderMenu: function (ul, items) {
        var that = this;
        ul.attr("class", "nav nav-pills nav-stacked  bs-autocomplete-menu");
        $.each(items, function (index, item) {
            that._renderItemData(ul, item);
        });
    },

    _resizeMenu: function () {
        var ul = this.menu.element;
        ul.outerWidth(Math.min(
            // Firefox wraps long text (possibly a rounding bug)
            // so we add 1px to avoid the wrapping (#7513)
            ul.width("").outerWidth() + 1,
            this.element.outerWidth()
        ));
    }

});

(function () {
    "use strict";
    var cities = [{
        "id": 1,
        "cityName": "Amsterdam"
    }, {
        "id": 2,
        "cityName": "Athens"
    }, {
        "id": 3,
        "cityName": "Baghdad"
    }, {
        "id": 4,
        "cityName": "Bangkok"
    }, {
        "id": 5,
        "cityName": "Barcelona"
    }, {
        "id": 6,
        "cityName": "Beijing"
    }, {
        "id": 7,
        "cityName": "Belgrade"
    }, {
        "id": 8,
        "cityName": "Berlin"
    }, {
        "id": 9,
        "cityName": "Bogota"
    }, {
        "id": 10,
        "cityName": "Bratislava"
    }, {
        "id": 11,
        "cityName": "Brussels"
    }, {
        "id": 12,
        "cityName": "Bucharest"
    }, {
        "id": 13,
        "cityName": "Budapest"
    }, {
        "id": 14,
        "cityName": "Buenos Aires"
    }, {
        "id": 15,
        "cityName": "Cairo"
    }, {
        "id": 16,
        "cityName": "CapeTown"
    }, {
        "id": 17,
        "cityName": "Caracas"
    }, {
        "id": 18,
        "cityName": "Chicago"
    }, {
        "id": 19,
        "cityName": "Copenhagen"
    }, {
        "id": 20,
        "cityName": "Dhaka"
    }, {
        "id": 21,
        "cityName": "Dubai"
    }, {
        "id": 22,
        "cityName": "Dublin"
    }, {
        "id": 23,
        "cityName": "Frankfurt"
    }, {
        "id": 24,
        "cityName": "Geneva"
    }, {
        "id": 25,
        "cityName": "Hanoi"
    }, {
        "id": 26,
        "cityName": "Helsinki"
    }, {
        "id": 27,
        "cityName": "Hong Kong"
    }, {
        "id": 28,
        "cityName": "Istanbul"
    }, {
        "id": 29,
        "cityName": "Jakarta"
    }, {
        "id": 30,
        "cityName": "Jerusalem"
    }, {
        "id": 31,
        "cityName": "Johannesburg"
    }, {
        "id": 32,
        "cityName": "Kabul"
    }, {
        "id": 33,
        "cityName": "Karachi"
    }, {
        "id": 34,
        "cityName": "Kiev"
    }, {
        "id": 35,
        "cityName": "Kuala Lumpur"
    }, {
        "id": 36,
        "cityName": "Lagos"
    }, {
        "id": 37,
        "cityName": "Lahore"
    }, {
        "id": 38,
        "cityName": "Lima"
    }, {
        "id": 39,
        "cityName": "Lisbon"
    }, {
        "id": 40,
        "cityName": "Ljubljana"
    }, {
        "id": 41,
        "cityName": "London"
    }, {
        "id": 42,
        "cityName": "Los Angeles"
    }, {
        "id": 43,
        "cityName": "Luxembourg"
    }, {
        "id": 44,
        "cityName": "Madrid"
    }, {
        "id": 45,
        "cityName": "Manila"
    }, {
        "id": 46,
        "cityName": "Marrakesh"
    }, {
        "id": 47,
        "cityName": "Melbourne"
    }, {
        "id": 48,
        "cityName": "Mexico City"
    }, {
        "id": 49,
        "cityName": "Montreal"
    }, {
        "id": 50,
        "cityName": "Moscow"
    }, {
        "id": 51,
        "cityName": "Mumbai"
    }, {
        "id": 52,
        "cityName": "Nairobi"
    }, {
        "id": 53,
        "cityName": "New Delhi"
    }, {
        "id": 54,
        "cityName": "NewYork"
    }, {
        "id": 55,
        "cityName": "Nicosia"
    }, {
        "id": 56,
        "cityName": "Oslo"
    }, {
        "id": 57,
        "cityName": "Ottawa"
    }, {
        "id": 58,
        "cityName": "Paris"
    }, {
        "id": 59,
        "cityName": "Prague"
    }, {
        "id": 60,
        "cityName": "Reykjavik"
    }, {
        "id": 61,
        "cityName": "Riga"
    }, {
        "id": 62,
        "cityName": "Rio de Janeiro"
    }, {
        "id": 63,
        "cityName": "Rome"
    }, {
        "id": 64,
        "cityName": "Saint Petersburg"
    }, {
        "id": 65,
        "cityName": "San Francisco"
    }, {
        "id": 66,
        "cityName": "Santiago de Chile"
    }, {
        "id": 67,
        "cityName": "São Paulo"
    }, {
        "id": 68,
        "cityName": "Seoul"
    }, {
        "id": 69,
        "cityName": "Shanghai"
    }, {
        "id": 70,
        "cityName": "Singapore"
    }, {
        "id": 71,
        "cityName": "Sofia"
    }, {
        "id": 72,
        "cityName": "Stockholm"
    }, {
        "id": 73,
        "cityName": "Sydney"
    }, {
        "id": 74,
        "cityName": "Tallinn"
    }, {
        "id": 75,
        "cityName": "Tehran"
    }, {
        "id": 76,
        "cityName": "The Hague"
    }, {
        "id": 77,
        "cityName": "Tokyo"
    }, {
        "id": 78,
        "cityName": "Toronto"
    }, {
        "id": 79,
        "cityName": "Venice"
    }, {
        "id": 80,
        "cityName": "Vienna"
    }, {
        "id": 81,
        "cityName": "Vilnius"
    }, {
        "id": 82,
        "cityName": "Warsaw"
    }, {
        "id": 83,
        "cityName": "Washington D.C."
    }, {
        "id": 84,
        "cityName": "Wellington"
    }, {
        "id": 85,
        "cityName": "Zagreb"
    }];

    $('.bs-autocomplete').each(function () {
        var _this = $(this),
            _data = _this.data(),
            _hidden_field = $('#' + _data.hidden_field_id);

        _this.after('<div class="bs-autocomplete-feedback form-control-feedback"><div class="loader">Loading...</div></div>')
            .parent('.form-group').addClass('has-feedback');

        var feedback_icon = _this.next('.bs-autocomplete-feedback');
        feedback_icon.hide();

        _this.autocomplete({
            minLength: 2,
            autoFocus: true,

            source: function (request, response) {
                var _regexp = new RegExp(request.term, 'i');
                var data = cities.filter(function (item) {
                    return item.cityName.match(_regexp);
                });
                response(data);
            },

            search: function () {
                feedback_icon.show();
                _hidden_field.val('');
            },

            response: function () {
                feedback_icon.hide();
            },

            focus: function (event, ui) {
                _this.val(ui.item[_data.item_label]);
                event.preventDefault();
            },

            select: function (event, ui) {
                _this.val(ui.item[_data.item_label]);
                _hidden_field.val(ui.item[_data.item_id]);
                event.preventDefault();
            }
        })
            .data('ui-autocomplete')._renderItem = function (ul, item) {
                return $('<li></li>')
                    .data("item.autocomplete", item)
                    .append('<a>' + item[_data.item_label] + '</a>')
                    .appendTo(ul);
            };
        // end autocomplete
    });
})();