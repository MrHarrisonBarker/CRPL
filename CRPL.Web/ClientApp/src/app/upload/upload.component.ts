import {Component, OnInit} from '@angular/core';
import {WorksService} from "../_Services/works.service";
import {HttpEventType} from "@angular/common/http";
import {DownloadFile} from "../utils";

@Component({
  selector: 'file-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit
{
  public CurrentFile: File = null as any;
  public CurrentProgress: number = 0;
  public FinishedUpload: boolean = false;
  public WorkHash: string = "";
  UploadError: string = "";
  DownloadError: string = "";

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
      this.worksService.UploadWork(this.CurrentFile).subscribe(event =>
      {
        if (event.type == HttpEventType.UploadProgress)
        {
          this.CurrentProgress = Math.round((event.loaded / event.total) * 100);
        } else if (event.type == HttpEventType.Response)
        {
          this.CurrentProgress = 100;
          this.FinishedUpload = true;
          this.WorkHash = event.body;
          console.log("Current work hash", this.WorkHash);
        }
      }, error => {
        console.log(error);
        this.UploadError = error.error
      });
    }
  }

  GetSignedWork ()
  {
    if (this.FinishedUpload && this.WorkHash)
    {
      this.worksService.GetSignedWork(this.WorkHash).subscribe(data =>
      {
        DownloadFile(this.CurrentFile.name, data);
        console.log(data);
      }, error => this.DownloadError = error.error);
    }
  }
}
