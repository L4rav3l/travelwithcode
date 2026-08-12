import React, { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { IoMdArrowBack } from "react-icons/io";

function Add()
{
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [admin, setAdmin] = useState(false);

    const token = localStorage.getItem("token") || sessionStorage.getItem("token");

    const navigate = useNavigate();

    const handleCreate = async () => {
        try
        {
            const response = await axios.post("/api/admin/create_user", {Username: username, Password: password, Admin: admin}, {headers: {Authorization: `Bearer ${token}`}});
        }
        catch
        {

        }
    }

    return (
        <div class="min-h-screen flex justify-center items-center">
            <div class="flex flex-col bg-white/10 w-96 p-2 rounded-lg gap-2">

                <div class="grid grid-cols-3 items-center w-full"> 
                    <div class="justify-self-start ">
                        <IoMdArrowBack class="text-white cursor-pointer" size={18} onClick={() => {navigate("../")}}/>
                    </div>
                
                    <div class="items-center">
                        <span class="text-white text-xl">Add user</span>
                    </div>
                </div>

                <div class="flex flex-col">
                    <span class="text-white">Username</span>
                    <input class="bg-white/10 p-1 rounded-lg text-white" onChange={(e) => setUsername(e.target.value)} value={username}/>
                </div>

                <div class="flex flex-col">
                    <span class="text-white">Password</span>
                    <input class="bg-white/10 p-1 rounded-lg text-white" onChange={(e) => setPassword(e.target.value)} value={password} />
                </div>

                <div class="flex flex-row gap-2">
                    <input type="checkbox" onChange={(e) => {setAdmin(e.target.checked)}} value={admin} />
                    <span class="text-white">Admin</span>
                </div>

                <button class="bg-white/10 p-2 rounded-lg text-white text-semibold hover:[transform:scale(1.01)]" onClick={() => handleCreate()}>Add User</button>

            </div>
        </div>
    )
}

export default Add;