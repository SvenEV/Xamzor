function xamzorInvokeCSharpMethod(typeName, methodName, args) {
    var methodOptions = {
        type: {
            assembly: 'Xamzor',
            name: typeName
        },
        method: {
            name: methodName
        }
    };

    Blazor.invokeDotNetMethod(methodOptions, args);
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

Blazor.registerFunction('Xamzor.measureImageAsync', source => new Promise((resolve, reject) => {
    var img = document.createElement('img');
    img.style = "visibility: collapse";

    img.onload = () => {
        resolve(img.naturalWidth + ',' + img.naturalHeight);
        document.body.removeChild(img);
    };

    img.onerror = () => {
        reject();
        document.body.removeChild(img);
    };

    // TODO: Remove trimming if strings are correctly passed without extra quotes
    img.src = source.substring(1, source.length - 1);
    document.body.appendChild(img);
}));

window.addEventListener('resize', () =>
    xamzorInvokeCSharpMethod('Xamzor.UI.Application', 'JSNotifyWindowResized'));