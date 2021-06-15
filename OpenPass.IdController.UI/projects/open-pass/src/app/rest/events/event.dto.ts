import { EventTypes } from '@enums/event-types.enum';

export class EventDto {
  eventType: EventTypes;
  originHost: string;
}
