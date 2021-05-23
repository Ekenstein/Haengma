import { Move } from './models/Models'
import * as SignalR from '@microsoft/signalr'

export interface IBoardClient {
    moveAdded(move: Move): void;
}

export class BoardController {
    private readonly hub: SignalR.HubConnection;
    private readonly client: IBoardClient;

    constructor(hubConnection: SignalR.HubConnection, client: IBoardClient) {
        this.hub = hubConnection;
        this.client = client;

        hubConnection.on("MoveAdded", addedMove => this.client.moveAdded(addedMove));
    }

    addMove(move: Move): void {
        this.hub.invoke("AddMove", move);
    }
}