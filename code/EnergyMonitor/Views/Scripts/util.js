function util_templateToChartXml(templateID, data) {
    var templateStr = $("#" + templateID).html();
    templateStr = templateStr.replace(/</g, "@#").replace(/>/g, "#@");
    var xmlStr = $("<div></div>").html($.tmpl(templateStr, data)).html();
    xmlStr = xmlStr.replace(/@#/g, "<").replace(/#@/g, ">");
    return xmlStr;
}
function util_index(item, array) {
    return $.inArray(item, array) + 1;
}