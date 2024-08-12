import { User } from "../models";

export class RegistrationResponse {
  status: number;
  message: string;
  user? = {} as User;
  users = {} as User[];

  constructor() {}
}
