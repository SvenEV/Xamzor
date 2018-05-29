function xamzorInvokeCSharpMethod(namespace, typeName, methodName, args) {
    const assemblyName = 'Xamzor';
    const method = Blazor.platform.findMethod(assemblyName, namespace, typeName, methodName);
    var csArgs = [];
    if (args)
        args.forEach(arg => csArgs.push(Blazor.platform.toDotNetString(arg)));
    let resultAsDotNetString = Blazor.platform.callMethod(method, null, csArgs);
}

Blazor.registerFunction('Xamzor.layout', (elem, x, y, w, h) => {
    if (!elem)
        return;

    elem.style.left = x + 'px';
    elem.style.top = y + 'px';
    elem.style.width = w + 'px';
    elem.style.height = h + 'px';
});

Blazor.registerFunction('Xamzor.getSize', elem => {
    var bounds = elem.parentElement.getBoundingClientRect();
    return bounds.width + "," + bounds.height;
});

Blazor.registerFunction('Xamzor.measureHtml', (element, maxWidth, maxHeight) => {
    var oldWidth = element.style.width;
    var oldHeight = element.style.height;
    var oldPosition = element.style.position;

    element.style.width = null;
    element.style.height = null;
    element.style.position = 'fixed';
    element.style.maxWidth = maxWidth ? maxWidth + 'px' : null;
    element.style.maxHeight = maxHeight ? maxHeight + 'px' : null;
    var bounds = element.getBoundingClientRect();
    var result = bounds.width + "," + bounds.height;

    element.style.width = oldWidth;
    element.style.height = oldHeight;
    element.style.position = oldPosition;

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
            [ source, img.naturalWidth + ',' + img.naturalHeight ]);
    }
});

window.addEventListener('resize', () =>
    xamzorInvokeCSharpMethod('Xamzor.UI', 'Application', 'JSNotifyWindowResized'));