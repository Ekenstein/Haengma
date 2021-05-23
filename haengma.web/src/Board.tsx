import { Goban } from 'react-goban';
import React, { useEffect, useState } from 'react';
import { Color, Point, GameId, Move } from './models/Models'
import { HubConnection, HubConnectionBuilder, HttpTransportType } from '@microsoft/signalr';

export interface Stone {
    color: Color;
    point: Point;
}

type BoardProperties = {
    boardSize: number,
    stones: Stone[],
    nextToPlay: Color
}

function convertPointToString(point: Point): string {
    var x = String.fromCharCode(65 + (point.x - 1))
    return x + point.y;
}

function convertColorToString(color: Color): string {
    switch (color) {
        case Color.Black: return "black";
        case Color.White: return "white";
    }
}

export function Board() {
    const [hubConnection, setHubConnection] = useState<HubConnection>();

    useEffect(() => {
        const createHubConnection = async () => {
            const hubConnection = new HubConnectionBuilder()
                .withUrl("https://localhost:44352/hub/goban", { skipNegotiation: true, transport: HttpTransportType.WebSockets })
                .build();

            try {
                await hubConnection.start();
                console.log("successfully connected");
            } catch(error) {
                alert(error);
            }
            setHubConnection(hubConnection);
        }

        createHubConnection();
    }, []);

    hubConnection.on("MoveAdded", (move: Move) => {

    })

    const onIntersectionClicked = async (intersection: string) => {
        var move: Move = {
            color: Color.Black,
            gameId: { id: "hejsan" },
            point: { x: 3, y: 3 }
        }
        try {
            await hubConnection.invoke("AddMove", move);
        } catch (error) {
            alert(error);
        }
    }

    return (
        <Goban size={19} stones={{}} nextToPlay={"black"} onIntersectionClick={onIntersectionClicked} />
    );
}

// export function Board(): FC<BoardProperties> = ({ boardSize, stones, nextToPlay }): ReactElement => {
//     useEffect(() => {

//     })
//     const dic = stones.reduce((map, stone) => {
//         var point = convertPointToString(stone.point);
//         var color = convertColorToString(stone.color);
//         map[point] = color;
//         return map;
//     }, {});
//     return (
//         <Goban 
//             size={boardSize} 
//             stones={dic}
//             nextToPlay={convertColorToString(nextToPlay)}
//             onIntersectionClick={(intersection => console.log(intersection))}
//         />
//     );
// }