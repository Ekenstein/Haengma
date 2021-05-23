import { HttpTransportType, HubConnection, HubConnectionBuilder } from "@microsoft/signalr"
import { useEffect, useState } from "react"
import { Board } from "./Board";
import { useInput } from "./hooks/InputHooks"

export const Lobby = () => {
    const [hubConnection, setHubConnection] = useState<HubConnection>();
    const [isConnected, setConnected] = useState(false);
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

    const submitUserInfo = async (userInfo: UserInfo) => {
        try {
            await hubConnection.invoke("AddConnectedUser", {
                name: userInfo.name,
                rank: {
                    rank: userInfo.rank,
                    rankType: userInfo.rankType
                }
            });

            setConnected(true);
        } catch (error) {
            console.log(error);
        }
    }

    return (
        <>
        {!isConnected &&
            <UserInfoForm onSubmit={submitUserInfo} />
        }
        </>
    )
}

enum RankType { Dan, Kyu }

type UserInfo = {
    name: string,
    rank: number,
    rankType: RankType
}

const UserInfoForm = props => {
    const { value, bind, reset } = useInput("");

    const handleSubmit = (evt) => {
        evt.preventDefault();
        props.onSubmit({
            name: value,
            rank: 10,
            rankType: RankType.Dan
        });
    }

    return (
        <form onSubmit={handleSubmit}>
            <label>
                Name:
                <input type="text" {...bind} />
            </label>
        
            <input type="submit" value="Submit" />
        </form>
    )
}