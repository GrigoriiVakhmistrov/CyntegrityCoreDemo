function showConfirmDialog(e) {
    if (confirm('Are you sure?')) {
    } else {
        e.preventDefault()
    }
}

$(function () {
    const hubConnection = new signalR.HubConnectionBuilder()
        .withUrl("/pipelinehub")
        .build();

    hubConnection.on("pipeline", data => {
        let $message = $(
            '<div class="fade show alert alert-info">\n' +
            '  <button class="close" type="button" data-dismiss="alert">×</button>' +
            ' Pipeline <b>' + data.pipeline.name + '</b> was recalulated. Average time: ' + data.calculatedTime + ' seconds\n' +
            '</div>');
        $('#messages').append($message);
    });

    hubConnection.start();
})