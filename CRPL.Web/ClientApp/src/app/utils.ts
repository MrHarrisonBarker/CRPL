export function isEmptyInputValue (value: any): boolean
{
  return value == null || value.length === 0;
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
