# Text Dungeon Crawler

A browser-based, text-first RPG dungeon crawler built with Next.js + TypeScript.
The game focuses on turn-based combat, party strategy, and custom character progression systems that go beyond traditional linear stat leveling.

## Project Overview

This project is currently a fully client-side game implementation, including:
- core game logic
- battle and turn-order systems
- leveling/progression mechanics
- in-memory state management

There is no backend yet; all data lives in the client runtime.

## Gameplay Overview

At a high level, gameplay is centered around:
- **Text-driven adventure flow**: progression through story and encounters via UI and text interactions.
- **Turn-based combat**: party members and enemies act in turn order with abilities, effects, and tactical choices.
- **Party management**: multiple characters, role-based decisions, and equipment/skill interactions.
- **Progression systems**: character growth is not only raw stat inflation, but also ability/equipment synergies and build direction.

## Core Features

- Turn-based battle loop with action selection and combat resolution
- Party and enemy entity systems
- Character progression and leveling logic
- Item/equipment and skill foundations
- Story and battle views in a Next.js App Router structure
- Shared game state via React context

## Technical Stack (Current)

- **Framework**: Next.js 15 (App Router)
- **Language**: TypeScript
- **UI Runtime**: React 19
- **State Management**: Client-side React context + local game logic libraries
- **Tooling**: ESLint, TypeScript compiler

## Planned Stack (Roadmap)

- **Backend Language**: C#
- **Backend Framework**: ASP.NET Core Web API (likely)
- **Responsibilities of Backend**:
	- persistence and save data
	- player accounts
	- long-term progression storage
	- server-side data validation and game session support

## Architecture Overview

### Current Architecture (Client-Only)

- `src/app/*`: route-level pages for gameplay flows (story, battle, party, results, etc.)
- `src/components/*`: reusable UI components (cards, overlays, bars, modals)
- `src/context/gameContext.tsx`: shared client game state
- `src/lib/*`: core domain/gameplay logic (combat, AI, entities, items, skills, storylines)

Current data flow is UI-driven and in-memory:
1. User input triggers actions from page/components.
2. Actions call game logic utilities in `src/lib`.
3. Game context/state updates and re-renders UI.

### Planned Architecture (Client + Backend)

- **Frontend (Next.js)** remains responsible for rendering, UX, and local interaction handling.
- **Backend (ASP.NET Core API)** handles persistence, identity/account data, and authoritative game data operations.
- **API boundary** introduces save/load endpoints and account-linked player progression.

## Installation

### Prerequisites

- Node.js 18+ (recommended: latest LTS)
- npm 9+

### Setup

```bash
npm install
```

## Run Locally

Start the development server:

```bash
npm run dev
```

Open:
- http://localhost:3000

Useful scripts:

```bash
npm run lint
npm run build
npm run start
```

## Folder Structure

```text
src/
	app/                   # Next.js routes/pages (story, battle, party, game flow)
	components/            # Shared UI components
	context/               # Global React context (game state)
	lib/
		obj/                 # Core object models (entities, items, skills)
		prefabs/             # Predefined enemy configurations
		storylines/          # Storyline data and types
		*.ts                 # Combat, AI, utility, and state helper logic
docs/                    # Design notes, drafts, and planning documents
public/                  # Static assets (fonts, etc.)
```

## Roadmap

### Near-Term

- Stabilize current gameplay loops and improve balance
- Expand storylines, encounters, and content variety
- Improve UI clarity and turn/battle feedback

### Mid-Term

- Define backend API contracts from current client logic
- Introduce C# ASP.NET Core backend project
- Add persistent save/load support

### Long-Term

- Add player accounts and profile-linked progression
- Move key persistence and validation to backend services
- Prepare for broader feature expansion (cloud saves, multi-device continuity)

## Future Improvements

- Automated tests for core battle/progression systems
- Data-driven content pipelines for enemies/items/skills/story events
- Enhanced combat log and analytics/debug tooling
- Improved onboarding/tutorial flow for new players
- Performance and state-transition profiling for larger game states

## Status

This project is actively evolving from a complete client-side prototype into a fuller game platform with backend support.

