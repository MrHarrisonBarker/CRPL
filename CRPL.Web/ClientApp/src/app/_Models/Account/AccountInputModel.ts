import {DOB} from "./DOB";

export interface AccountInputModel
{
  FirstName?: string;
  LastName?: string;
  DateOfBirth?: DOB;
  RegisteredJurisdiction?: string;
  Email?: string;
  DialCode?: string;
  PhoneNumber?: string;
}
