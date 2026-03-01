export interface DialogueEntry {
  source: string;
  dialogue: string;
}

export interface Option {
  title: string;
  ref: string;
}

export interface FightConfig {
  enemies: string[];
  "ai-target": string;
  "next-scene": boolean;
  ref?: string;
}

export interface ContentBlock {
  order: number;
  chat: DialogueEntry[];
  options?: Option[];
  acquire?: string;
  effect?: string;
  require?: string;
  fight?: FightConfig;
}

export interface Scene {
  name: string;
  sequence: number;
  content: Record<string, ContentBlock>;
}

type StorylineData = Scene[];

export default StorylineData;