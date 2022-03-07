import {AbstractControl} from "@angular/forms";

export function isEmptyInputValue (value: any): boolean
{
  return value == null || value.length === 0;
}

export function isInputValid (control: AbstractControl)
{
  return control.valid && !isEmptyInputValue(control.value)
}

export function DownloadFile (fileName: string, data: Blob)
{
  const a = document.createElement('a');
  a.setAttribute('style', 'display:none;');
  document.body.appendChild(a);
  a.download = fileName;
  a.href = URL.createObjectURL(data);
  a.target = '_blank';
  a.click();
  document.body.removeChild(a);
}

export function syntaxHighlight(json: string) {
  json = json.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
  return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function (match) {
    var cls = 'number';
    if (/^"/.test(match)) {
      if (/:$/.test(match)) {
        cls = 'key';
      } else {
        cls = 'string';
      }
    } else if (/true|false/.test(match)) {
      cls = 'boolean';
    } else if (/null/.test(match)) {
      cls = 'null';
    }
    return '<span class="' + cls + '">' + match + '</span>';
  }).replace(/([{},\]\[])+/g, match => '<span class="punctuation">' + match + '</span>');
}
