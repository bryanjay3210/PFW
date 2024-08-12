import { User } from "../models";

export class LoginResponse {
  status: number;
  message: string;
  token: string;
  user? = {} as User;

  constructor() {}
}
