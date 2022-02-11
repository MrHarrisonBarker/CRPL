import {Component, Input, OnInit} from '@angular/core';
import {CopyrightRegistrationViewModel} from "../../_Models/Applications/CopyrightRegistrationViewModel";
import {WorkType} from "../../_Models/WorkType";
import {ClrTimelineStepState} from "@clr/angular";

@Component({
  selector: 'cpy-registration-view [Application]',
  templateUrl: './cpy-registration-view.component.html',
  styleUrls: ['./cpy-registration-view.component.css']
})
export class CpyRegistrationViewComponent implements OnInit
{
  @Input() Application!: CopyrightRegistrationViewModel;
  public timelineSuccess: ClrTimelineStepState = ClrTimelineStepState.SUCCESS;
  public timelineProcessing: ClrTimelineStepState = ClrTimelineStepState.PROCESSING;
  public timelineNot: ClrTimelineStepState = ClrTimelineStepState.NOT_STARTED;
  public timelineCurrent: ClrTimelineStepState = ClrTimelineStepState.CURRENT;

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

  get Meta()
  {
    let meta = {
      Title: this.Application.Title,
      WorkType: WorkType[this.Application.WorkType],
      WorkHash: this.Application.WorkHash,
      WorkUri: this.Application.WorkUri,
      Legal: this.Application.Legal,
      OwnershipStructure: this.Application.OwnershipStakes,
      ProtectionLength: this.Application.YearsExpire
    };

    return syntaxHighlight(JSON.stringify(meta, undefined, 4)).replace(/([{},\]\[])+/g, match => '<span class="punctuation">' + match + '</span>');
  }

  get WorkStatus()
  {
    return this.Application.AssociatedWork?.Status;
  }
}

function syntaxHighlight(json: string) {
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
  });
}
