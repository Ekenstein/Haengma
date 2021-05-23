export interface GameId {
    id: string;
}

export enum Color { Black, White }

export interface Point {
    x: number;
    y: number;
}

export interface Move {
    gameId: GameId,
    color: Color,
    point: Point
}