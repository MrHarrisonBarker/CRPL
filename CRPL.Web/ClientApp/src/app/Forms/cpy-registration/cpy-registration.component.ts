import {Component, Input, OnInit} from '@angular/core';
import {FormBuilder, FormGroup} from "@angular/forms";
import {AuthService} from "../../_Services/auth.service";
import {OwnershipStake, OwnershipStakeInput} from "../../_Models/StructuredOwnership/OwnershipStake";
import {FormsService} from "../../_Services/forms.service";
import {WorkType} from "../../_Models/WorkType";
import {CopyrightType} from "../../_Models/CopyrightType";
import {CopyrightRegistrationInputModel} from "../../_Models/Applications/CopyrightRegistrationInputModel";

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
  public WorkTypes: string[] = Object.values(WorkType).filter(value => typeof value != 'number') as string[];
  public OwnershipStakes: OwnershipStakeInput[] = [];

  constructor (private formBuilder: FormBuilder, public authService: AuthService, private formsService: FormsService)
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

  public ngOnInit (): void
  {
    this.selectRights(this.StandardRights);
  }

  private selectRights (rights: string[]): void
  {
    this.RegistrationForm.controls['Rights'].reset();
    for (let right in rights)
    {
      let rightsGroup = this.RegistrationForm.controls['Rights'] as FormGroup;
      rightsGroup.controls[this.StandardRights[right]].setValue(true);
    }
  }

  public ChangeCopyrightType (): void
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

  public ChangeWorkType (): void
  {

  }

  public Update (): void
  {
    let inputModel: CopyrightRegistrationInputModel = {
      Title: this.RegistrationForm.value.Title,
      WorkUri: this.RegistrationForm.value.WorkUri,
      Id: this.ExistingApplication != undefined ? this.ExistingApplication : undefined,
      OwnershipStakes: this.OwnershipStakes.map(x => ({Owner: x.Owner, Share: x.Share}) as OwnershipStake),
      WorkType: this.RegistrationForm.value.WorkType,
      Expires: this.RegistrationForm.value.Expires,
      CopyrightType: this.RegistrationForm.value.ProtectionType
    }

    console.log(inputModel);

    // this.formsService.UpdateCopyrightRegistration().subscribe(x => console.log("updated copyright registration form", x))
  }
}
