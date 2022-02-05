import {Component, Input, OnInit} from '@angular/core';
import {AbstractControl, FormBuilder, FormGroup} from "@angular/forms";

interface RightMeta
{
  Name: string;
  Description: string;
}

@Component({
  selector: 'cpy-registration',
  templateUrl: './cpy-registration.component.html',
  styleUrls: ['./cpy-registration.component.css']
})
export class CpyRegistrationComponent implements OnInit
{
  @Input() ExistingApplication: string | undefined;
  public RegistrationForm: FormGroup;
  public AcceptedUla: boolean = false;

  public Rights: RightMeta[] = [
    {Name: "authorship", Description: "The eternal right to original authorship."},
    {
      Name: "reproduce",
      Description: "The exclusive right to reproduction and the right to authorise any other party for reproduction."
    },
    {
      Name: "distribution",
      Description: "The exclusive right to distribution and the right to authorise any other party for distribution including but not limited to; online distribution markets, public performances or broadcasts."
    },
    {
      Name: "adapt",
      Description: "The exclusive right to adapt and recite work while maintaining all rights to the derivative including the right to seek additional protection on the derivative works."
    },
    {
      Name: "changeOfOwnership",
      Description: "The exclusive right to alter the underlying ownership of the work in- cluding removing the original party as principle owner of the work."
    },
  ];

  public StandardRights: string[] = ["authorship", "reproduce", "distribution", "adapt", "changeOfOwnership"];
  public PermissiveRights: string[] = ["authorship"];
  public CopyleftRights: string[] = ["authorship", "reproduce"];
  public WorkTypes: string[] = ["Image", "Video", "Sound", "PDF"];

  constructor (private formBuilder: FormBuilder)
  {
    this.RegistrationForm = formBuilder.group({
      Title: [''],
      WorkHash: [''],
      WorkUri: [''],
      Legal: [''],
      Expires: [100],
      ProtectionType: ['Standard'],
      Rights: formBuilder.group({
        authorship: [false],
        reproduce: [false],
        distribution: [false],
        adapt: [false],
        changeOfOwnership: [false]
      }),
      WorkType: ['Image']
    });
  }

  ngOnInit (): void
  {
    this.selectRights(this.StandardRights);
  }

  private selectRights (rights: string[])
  {
    this.RegistrationForm.controls['Rights'].reset();
    for (let right in rights)
    {
      let rightsGroup = this.RegistrationForm.controls['Rights'] as FormGroup;
      rightsGroup.controls[this.StandardRights[right]].setValue(true);
    }
  }

  public ChangeCopyrightType ()
  {
    switch (this.RegistrationForm.value.ProtectionType)
    {
      case "Standard":
        this.selectRights(this.StandardRights);
        break;
      case "Permissive":
        this.selectRights(this.PermissiveRights);
        break;
      case "Copyleft":
        this.selectRights(this.CopyleftRights);
        break;
    }
  }

  ChangeWorkType ()
  {

  }
}
