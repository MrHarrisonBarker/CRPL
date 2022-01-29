let userPaths : UserPaths = {
  EmailExists: "", PhoneExists: "",
  Account: "", Auth: "", AuthenticateSignature: "", FetchNonce: "", RevokeAuthenticate: "", Update: "",
  BasePath: "user"
}

let worksPaths: WorksPaths = {
  BasePath: "works"
}

userPaths = {
  ...userPaths,
  FetchNonce: userPaths.BasePath + "/nonce",
  AuthenticateSignature: userPaths.BasePath + "/sig",
  Auth: userPaths.BasePath + "/auth",
  Account: userPaths.BasePath,
  PhoneExists: userPaths.BasePath + "/unique/phone",
  EmailExists: userPaths.BasePath + "/unique/email"
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
}

interface WorksPaths {
  readonly BasePath: string;
}

export const UserPaths: UserPaths = userPaths;
export const WorksPaths: WorksPaths = worksPaths;
