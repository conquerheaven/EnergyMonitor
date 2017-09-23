(function ($) {
    jQuery.fn.timepicker = function () {
        this.each(function () {
            var obj = $(this);

            // the options we need to generate
            var hrs = new Array('00', '01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23');
            var mins = new Array('00', '05', '10', '15', '20', '25', '30', '35', '40', '45', '50', '55');

            // build the new DOM objects
            var hourSelect = $('<select></select>');
            for (hr in hrs) {
                $('<option value="' + hrs[hr] + '">' + hrs[hr] + '</option>').appendTo(hourSelect);
            }
            var minSelect = $('<select></select>');
            for (mn in mins) {
                $('<option value="' + mins[mn] + '">' + mins[mn] + '</option>').appendTo(minSelect);
            }
            var warpperDiv = $('<div class="time-picker" ></div>').append(hourSelect).append(minSelect);
            warpperDiv.append($('<a class="confirm">&nbsp;</a>').click(function () {
                obj.val(hourSelect.val() + ":" + minSelect.val());
                warpperDiv.slideUp("fast");
            }));
            warpperDiv.append($('<a class="cancel">&nbsp;</a>').click(function () {
                obj.val("");
                warpperDiv.slideUp("fast");
            }));
            obj.after(warpperDiv);
            obj.click(function () {
                $("div.time-picker").hide();
                var marginLeftVal = obj.offset().left - obj.parent().offset().left;
                warpperDiv.css("margin-left", marginLeftVal);
                warpperDiv.slideDown("fast");
            });

        });
        return this;
    };
})(jQuery);
