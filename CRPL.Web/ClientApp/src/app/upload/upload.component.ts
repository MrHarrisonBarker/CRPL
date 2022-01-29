import {Component, OnInit} from '@angular/core';
import {WorksService} from "../_Services/works.service";

@Component({
  selector: 'file-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit
{
  public CurrentFile: File = null as any;

  constructor (private worksService: WorksService)
  {
  }

  ngOnInit (): void
  {
  }

  public onFileChange (fileInputEvent: Event): void
  {
    let files = (fileInputEvent.currentTarget as HTMLInputElement).files

    if (files != null && files.length > 0)
    {
      this.CurrentFile = files[0];
      console.log("the current file is now", this.CurrentFile);
    }
  }

  public Upload (): void
  {
    if (this.CurrentFile != null)
    {
      this.worksService.UploadWork(this.CurrentFile).subscribe();
    }
  }
}
