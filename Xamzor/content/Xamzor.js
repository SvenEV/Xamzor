function xamzorInvokeCSharpMethod(namespace, typeName, methodName, args) {
    const assemblyName = 'Xamzor';
    const method = Blazor.platform.findMethod(assemblyName, namespace, typeName, methodName);
    var csArgs = [];
    if (args)
        args.forEach(arg => csArgs.push(Blazor.platform.toDotNetString(arg)));
    let resultAsDotNetString = Blazor.platform.callMethod(method, null, csArgs);
}

Blazor.registerFunction('Xamzor.layout', (elem, x, y, w, h) => {
    elem.style.left = x + 'px';
    elem.style.top = y + 'px';
    elem.style.width = w + 'px';
    elem.style.height = h + 'px';
});

Blazor.registerFunction('Xamzor.getSize', elem => {
    var bounds = elem.parentElement.getBoundingClientRect();
    return bounds.width + "," + bounds.height;
});

Blazor.registerFunction('Xamzor.measureHtml', data => {
    var container = document.getElementById('xamzorMeasureContainer');

    if (container === null) {
        container = document.createElement('div');
        container.style = 'display: inline-block; visibility: hidden;';
        container.id = 'xamzorMeasureContainer';
        document.body.appendChild(container);
    }

    container.innerHTML = data;
    var bounds = container.getBoundingClientRect();
    var result = bounds.width + "," + bounds.height;
    container.innerHTML = "";
    return result;
});

Blazor.registerFunction('Xamzor.measureImage', source => {
    var img = document.createElement('img');
    img.style = "visibility: collapse";
    img.src = source;

    img.onload = function () {
        returnResult();
        document.body.removeChild(img);
    };

    img.onerror = function () {
        returnResult();
        document.body.removeChild(img);
    };

    document.body.appendChild(img);

    function returnResult() {
        xamzorInvokeCSharpMethod(
            'Xamzor.UI', 'ImageMeasureInterop', 'NotifyImageMeasured',
            source, img.naturalWidth + ',' + img.naturalHeight);
    }
});

window.addEventListener('resize', () =>
    xamzorInvokeCSharpMethod('Xamzor.UI', 'Application', 'JSNotifyWindowResized'));