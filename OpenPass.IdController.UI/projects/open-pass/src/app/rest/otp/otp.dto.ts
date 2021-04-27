export class OtpDto {
  email!: string;
  otp?: string;

  constructor(data?: OtpDto) {
    if (data) {
      this.email = data.email;
      this.otp = data.otp;
    }
  }
}
