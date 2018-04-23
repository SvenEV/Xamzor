function xamzorInvokeCSharpMethod(namespace, typeName, methodName, args) {
    const assemblyName = 'Xamzor';
    const method = Blazor.platform.findMethod(assemblyName, namespace, typeName, methodName);
    var csArgs = [];
    if (args)
        args.forEach(arg => csArgs.push(Blazor.platform.toDotNetString(arg)));
    let resultAsDotNetString = Blazor.platform.callMethod(method, null, csArgs);
}

window.addEventListener('resize', () =>
    xamzorInvokeCSharpMethod('Xamzor.UI', 'Application', 'JSNotifyWindowResized'));