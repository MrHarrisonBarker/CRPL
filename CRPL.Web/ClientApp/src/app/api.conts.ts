let userPaths : UserPaths = {
  Account: "", Auth: "", AuthenticateSignature: "", FetchNonce: "", RevokeAuthenticate: "", Update: "",
  BasePath: "user"
}

userPaths = {
  ...userPaths,
  FetchNonce: userPaths.BasePath + "/nonce",
  AuthenticateSignature: userPaths.BasePath + "/sig",
  Auth: userPaths.BasePath + "/auth"
}

interface UserPaths {
  readonly BasePath: string;
  readonly Auth: string;
  readonly RevokeAuthenticate: string;
  readonly FetchNonce: string;
  readonly AuthenticateSignature: string;
  readonly Account: string;
  readonly Update: string;
}

export const UserPaths: UserPaths = userPaths;
