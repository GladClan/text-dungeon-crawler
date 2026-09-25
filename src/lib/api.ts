import { Battle, Entity, EventResult, SceneEvent } from "./types";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5000";
type RequestOptions = {
    method?: "GET" | "POST" | "PUT" | "DELETE" | "PATCH";
    body?: unknown;
}

async function fetchJson<T>(path: string, options: RequestOptions = {}): Promise<T> {
    const {method = "GET", body} = options;

    const response = await fetch(`${API_BASE_URL}${path}`, {
        method: method,
        headers: {
            Accept: "Application/json",
            ...(body !== undefined ? { "Content-Type": "application/json" } : {}),
        },
        body: body !== undefined ? JSON.stringify(body) : undefined,
    });

    if (response.status >= 400){
        throw new Error(`API request failed: ${response.status} ${response.statusText}`)
    }

    if (response.status === 204){
        return undefined as T;
    }

    const raw = await response.text();
    if (!raw){
        return undefined as T;
    }

    return JSON.parse(raw) as T;
}

export async function GetCurrentEvent() {
    return fetchJson<SceneEvent>(`/api/events/get-current`);
}

export async function ChooseOption(optionId: number) {
    return fetchJson<EventResult>(`/api/events/choose-option/${optionId}`, {method: "PATCH"});
}

export async function SelectTarget(targetId: string) {
    return fetchJson<boolean>(`/api/events/select-target`, {method: "POST", body: targetId})
}

export async function GetParty(partyId: string) {
    const result = fetchJson<Entity[]>(`/api/entities/party/${partyId}`);
    return result;
}

export async function GetActiveBattle() {
    const result = fetchJson<Battle>(`/api/events/battle`);
    return result;
}

export async function UseItem(sourceId: string, itemId: string, targetIds: string[]) {
    // 
}