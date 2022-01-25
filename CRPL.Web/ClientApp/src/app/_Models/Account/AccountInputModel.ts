import {DOB} from "./DOB";

export interface AccountInputModel
{
  FirstName: string;
  LastName: string;
  DOB: DOB;
  RegisteredJurisdiction: string;
  Email: string;
  PhoneNumber: string;
}
