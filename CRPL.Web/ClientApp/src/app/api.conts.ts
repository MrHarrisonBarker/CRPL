let userPaths : UserPaths = {
  EmailExists: "", PhoneExists: "",
  Account: "", Auth: "", AuthenticateSignature: "", FetchNonce: "", RevokeAuthenticate: "", Update: "", Search: "",
  BasePath: "user"
}

let worksPaths: WorksPaths = {
  BasePath: "works"
}

let formsPaths: FormsPaths = {
  CopyrightRegistrationSubmit: "",
  CopyrightRegistration: "",
  BasePath: "forms"
}

formsPaths = {
  ...formsPaths,
  CopyrightRegistration: formsPaths.BasePath + "/copyright/registration",
  CopyrightRegistrationSubmit: formsPaths.BasePath + "/copyright/submit/registration"
}

userPaths = {
  ...userPaths,
  FetchNonce: userPaths.BasePath + "/nonce",
  AuthenticateSignature: userPaths.BasePath + "/sig",
  Auth: userPaths.BasePath + "/auth",
  Account: userPaths.BasePath,
  PhoneExists: userPaths.BasePath + "/unique/phone",
  EmailExists: userPaths.BasePath + "/unique/email",
  Search: userPaths.BasePath + "/search"
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
}

interface WorksPaths {
  readonly BasePath: string;
}

interface FormsPaths {
  readonly BasePath: string;
  readonly CopyrightRegistration: string;
  readonly CopyrightRegistrationSubmit: string;
}

export const UserPaths: UserPaths = userPaths;
export const WorksPaths: WorksPaths = worksPaths;
export const FormsPaths: FormsPaths = formsPaths;
