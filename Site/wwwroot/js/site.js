function showConfirmDialog(e) {
    if (confirm('Are you sure?')) {
    } else {
        e.preventDefault()
    }
}

$(function () {
    connectToHub()
})

function connectToHub() {
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
}

function updatePipelineName(pipeline) {
    $(document).on('input', '#name', function () {
        pipeline.name = $(this).val()
    })
}

function addTaskToPipeline(pipeline, taskId) {
    let addButton = $('input[value=' + taskId + ']').siblings('.btn-add')
    addButton.hide()

    let newElement = $(addButton).parent().clone()
    newElement.addClass('task-in-pipeline')
    newElement.addClass(taskId)
    $('.tasks-in-pipeline').append(newElement)

    let $removeButton = $('<button class="btn btn-primary btn-remove" type="button">Remove</button>')
    $removeButton.click({ pipeline: pipeline }, removeTaskFromPipeline)
    $('.' + taskId).append($removeButton)

    $(addButton).addClass(taskId)
    $(addButton).addClass('hided')
}

function removeTaskFromPipeline(e) {
    let pipeline = e.data.pipeline
    let taskId = $(this).siblings('input[name="_id"]').val()

    let index = pipeline.taskIds.indexOf(taskId)
    if (index >= 0) {
        pipeline.taskIds.splice(index, 1)

        let $addButton = $('.hided').filter('.' + taskId)
        $addButton.show()

        $addButton.removeClass(taskId)
        $addButton.removeClass('hided')

        $(this).parent().remove()
    }
}

function setAntiforgeryHeader() {
    $.ajaxSetup({
        beforeSend: request => {
            token = $('input[name="__RequestVerificationToken"]').val()
            request.setRequestHeader("RequestVerificationToken", token);
        }
    })
}

function onClickToAddButton(e) {
    let pipeline = e.data.pipeline
    let taskId = $(this).siblings('input[name="_id"]').val()

    if (pipeline.taskIds.indexOf(taskId) < 0) {
        pipeline.taskIds.push(taskId)
        addTaskToPipeline(pipeline, taskId)
    }
}

function savePipeline(pipeline, url, redirectUrl) {
    delete pipeline.tasks;

    $.post({
        url: url,
        data: pipeline
    })
        .done(function () {
            window.location.pathname = redirectUrl
        })
        .fail(function (error) {
            if (error.status === 200 && error.statusText === 'OK')
                window.location.pathname = redirectUrl
        })
}
