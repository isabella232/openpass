import { PostMessageActions } from '../enums/post-message-actions.enum';

export type PostMessagePayload = {
  action: PostMessageActions;
  token?: string;
};
