function JsonP(url) {

    $.ajax({
        type: 'GET',
        url: url,
        dataType: 'jsonp',
        success: function(data) {
            console.log('Success', data);
        },
        error: function(request, textStatus, errorThrown) {
            console.log('Error', request.responseText, textStatus, errorThrown);
        }
    });
    
}