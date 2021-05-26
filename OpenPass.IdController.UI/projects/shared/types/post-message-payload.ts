import { PostMessageActions } from '../enums/post-message-actions.enum';

export type PostMessagePayload = {
  action: PostMessageActions;
  ifaToken?: string;
  uid2Token?: string;
  height?: number;
  email?: string;
  isDeclined?: boolean;
};
