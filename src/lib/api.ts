import { Battle, Entity, EventResult, SceneEvent, TurnOver } from "./types";

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
    const result = fetchJson<TurnOver>(
        `/api/events/use-item`,
        {
            method: "PATCH",
            body: {
                sourceId: sourceId,
                actionId: itemId,
                targetIds: targetIds
            }
    });
    return result;
}

export async function UseSkill(sourceId: string, skillId: string, targetIds: string[]) {
    const result = fetchJson<TurnOver>(
        `/api/events/use-skill`,
        {
            method: "PATCH",
            body: {
                sourceId: sourceId,
                actionId: skillId,
                targetIds: targetIds
            }
    });
    return result;
}

export async function DefaultAttack(sourceId: string, targetId: string) {
    const result = fetchJson<TurnOver>(
        `/api/events/default-attack`,
        {
            method: "PATCH",
            body: {
                sourceId: sourceId,
                actionId: "default",
                targetIds: [targetId]
            }
    });
    return result;
}

export async function Defend(sourceId: string) {
    const result = fetchJson<TurnOver>(
        `/api/events/defend`,
        {
            method: "PATCH",
            body: {
                sourceId: sourceId,
                actionId: "defend",
                targetIds: [sourceId]
            }
    });
    return result;
}

export async function GetOpponentTurn(sourceId: string) {
    const result = fetchJson<TurnOver>(
        `/api/events/opponent-turn`,
        {
            method: "PATCH",
            body: sourceId
    });
    return result;
}