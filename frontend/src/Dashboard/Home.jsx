import axios from "axios";
import React, { useEffect, useState } from "react";

function Home()
{

    const [username, setUsername] = useState("");

    const token = localStorage.getItem("token") || sessionStorage.getItem("token");

    useEffect(() => {

        const handleUsername = async () => {
            try
            {
                const response = await axios.get("/api/profile/data", {headers: {Authorization: `Bearer ${token}`}});
                setUsername(response.data);
            }
            catch(ex)
            {
                console.log(ex);
            }
        };

        handleUsername();

    }, [])
 
    return (
        <>
            <div class="h-screen flex flex-col justify-center items-center">
                <div class="bg-white/10 p-2 rounded-lg">
                    <span class="text-xl text-white">Welcome back, {username}</span>
                </div>
            </div>
        </> 
    );

}

export default Home;