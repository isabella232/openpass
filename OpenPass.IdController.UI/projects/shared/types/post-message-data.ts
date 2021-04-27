import { PostMessageTypes } from '../enums/post-message-types.enum';
import { PostMessagePayload } from './post-message-payload';

export type PostMessageData = {
  type: PostMessageTypes;
  payload: PostMessagePayload;
};
