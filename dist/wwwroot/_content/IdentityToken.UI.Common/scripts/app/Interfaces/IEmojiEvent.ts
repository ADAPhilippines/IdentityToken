interface EmojiData {
   unicode: string;
}

interface IEmojiEvent extends Event {
    detail: EmojiData
}

export { IEmojiEvent, EmojiData }