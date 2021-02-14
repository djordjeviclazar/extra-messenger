import { MessageReturnDto } from "./messageReturnDto";

export interface ReceivedMessageDto {
  senderId?: string;
  receiverId?: string;
  chatInteractionId?: string;
  message?: MessageReturnDto;
  edited?: boolean;
  deleted?: boolean;
}
