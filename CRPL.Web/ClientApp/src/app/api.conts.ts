let userPaths : UserPaths = {
  IsReal: "",
  EmailExists: "", PhoneExists: "",
  Account: "", Auth: "", AuthenticateSignature: "", FetchNonce: "", RevokeAuthenticate: "", Update: "", Search: "",
  BasePath: "user"
}

let worksPaths: WorksPaths = {
  BasePath: "works"
}

let formsPaths: FormsPaths = {
  DeleteUser: "", TransferWallet: "",
  RecordPayment: "", ResolveDispute: "",
  Dispute: "", DisputeSubmit: "",
  Cancel: "",
  OwnershipRestructure: "", OwnershipRestructureSubmit: "",
  GetMy: "",
  CopyrightRegistrationSubmit: "",
  CopyrightRegistration: "",
  BasePath: "forms"
}

let copyrightPaths: CopyrightPaths = {Complete: "", Sync: "", Bind: "", BasePath: "copyright"}

let queryPaths: QueryPaths = {Disputes: "", Search: "", All: "", BasePath: "q", GetMy: ""}

formsPaths = {
  ...formsPaths,
  CopyrightRegistration: formsPaths.BasePath + "/copyright/registration",
  CopyrightRegistrationSubmit: formsPaths.BasePath + "/copyright/submit/registration",
  OwnershipRestructure: formsPaths.BasePath + "/copyright/ownership",
  OwnershipRestructureSubmit: formsPaths.BasePath + "/copyright/submit/ownership",
  Dispute: formsPaths.BasePath + "/copyright/dispute",
  DisputeSubmit: formsPaths.BasePath + "/copyright/submit/dispute",
  GetMy: formsPaths.BasePath + "/users",
  Cancel: formsPaths.BasePath,
  ResolveDispute: formsPaths.BasePath + "/copyright/resolve/dispute",
  RecordPayment: formsPaths.BasePath + "/copyright/record/payment/dispute",
  DeleteUser: formsPaths.BasePath + "/user",
  TransferWallet: formsPaths.BasePath + "/wallet"
}

userPaths = {
  ...userPaths,
  FetchNonce: userPaths.BasePath + "/nonce",
  AuthenticateSignature: userPaths.BasePath + "/sig",
  Auth: userPaths.BasePath + "/auth",
  Account: userPaths.BasePath,
  PhoneExists: userPaths.BasePath + "/unique/phone",
  EmailExists: userPaths.BasePath + "/unique/email",
  Search: userPaths.BasePath + "/search",
  IsReal: userPaths.BasePath + "/real"
}

copyrightPaths = {
  ...copyrightPaths,
  Bind: copyrightPaths.BasePath + "/bind",
  Sync: copyrightPaths.BasePath + "/sync",
  Complete: copyrightPaths.BasePath + "/complete"
}

queryPaths = {
  ...queryPaths,
  GetMy: queryPaths.BasePath + "/my",
  All: queryPaths.BasePath + "/all",
  Search: queryPaths.BasePath + "/search",
  Disputes: queryPaths.BasePath + "/disputes"
}

interface UserPaths {
  readonly BasePath: string;
  readonly Auth: string;
  readonly RevokeAuthenticate: string;
  readonly FetchNonce: string;
  readonly AuthenticateSignature: string;
  readonly Account: string;
  readonly Update: string;
  readonly PhoneExists: string;
  readonly EmailExists: string;
  readonly Search: string;
  readonly IsReal: string;
}

interface WorksPaths {
  readonly BasePath: string;
}

interface FormsPaths {
  readonly BasePath: string;
  readonly CopyrightRegistration: string;
  readonly CopyrightRegistrationSubmit: string;
  readonly OwnershipRestructure: string;
  readonly OwnershipRestructureSubmit: string;
  readonly Dispute: string;
  readonly DisputeSubmit: string;
  readonly ResolveDispute: string;
  readonly RecordPayment: string;
  readonly GetMy: string;
  readonly Cancel: string;
  readonly DeleteUser: string;
  readonly TransferWallet: string;
}

interface CopyrightPaths {
  readonly BasePath: string;
  readonly Bind: string;
  readonly Sync: string;
  readonly Complete: string;
}

interface QueryPaths {
  readonly Search: string;
  readonly BasePath: string;
  readonly GetMy: string;
  readonly All: string;
  readonly Disputes: string;
}

export const UserPaths: UserPaths = userPaths;
export const WorksPaths: WorksPaths = worksPaths;
export const FormsPaths: FormsPaths = formsPaths;
export const CopyrightPaths: CopyrightPaths = copyrightPaths;
export const QueryPaths: QueryPaths = queryPaths;
