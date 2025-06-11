$(document).ready(function () {

    function split(val) {
        return val.split(/,\s*/);
    }

    function extractLast(term) {
        return split(term).pop();
    }

    $('#tags')
        .on('keydown', function (event) {
            //if (event.key === ',') {
            //    var val = extractLast(this.value).trim();

            //    if (val) {
            //        createTagItemMark({
            //            id: val,
            //            name: val,
            //            type: 'Product'
            //        });

            //        this.value = '';
            //    }
            //}
        })
        .autocomplete({
            minLength: 1,
            source: function (request, response) {

                $.ajax({
                    url: 'https://localhost:7089/api/v1/TagAPIs/search',
                    data: {
                        keyword: extractLast(request.term),
                        type: 'Product'
                    },
                    success: function (data) {
                        console.log(data)

                        response(data);
                    }
                })
            },
            focus: function (event, ui) {
                return false;
            },
            select: function (event, ui) {
                createTagItemMark(ui.item);

                //var terms = split(this.value);
                //// remove the current input
                //terms.pop();
                //// add the selected item
                //terms.push(ui.item.name);
                //// add placeholder to get the comma-and-space at the end
                //terms.push("");

                //this.value = terms.join(", ");

                this.value = '';

                console.log(this.value);

                return false;
            }
        })
        .autocomplete("instance")._renderItem = function (ul, item) {
            return $("<li>")
                .append(`<div>${item.name}</div>`)
                .appendTo(ul);
        };

    $(document).on('click', '.btnDelTagItem', function () {
        $(this).parent().parent().remove();
    });

    function createTagItemMark(item) {
        var newSelectedItem = $('<div>', {
            class: 'tag-item-wrap'
        }).append(
            $('<div>', {
                class: 'tag-item'
            }).append(
                $('<span>').text(item.name),
                $('<input>', {
                    value: item.id,
                    name: 'Tags',
                    type: 'hidden'
                }),
                $('<span>', {
                    class: 'btnDelTagItem',
                    html: '<i class="fa-duotone fa-solid fa-xmark"></i>'
                })
            )
        )

        $('.tags-selected-wrap').append(newSelectedItem);
    }
})