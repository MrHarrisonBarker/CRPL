import {UserAccountViewModel} from "./UserAccountViewModel";
import {PartialField} from "../PartialField";

export interface UserAccountStatusModel
{
  UserAccount: UserAccountViewModel;
  PartialFields: PartialField[];
}
