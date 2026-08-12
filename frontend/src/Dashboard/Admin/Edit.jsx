import React, { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate, useSearchParams } from "react-router-dom";
import { IoMdArrowBack } from "react-icons/io";

function Edit()
{
    const [id, setId] = useState(0);
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [admin, setAdmin] = useState(false);

    const token = localStorage.getItem("token") || sessionStorage.getItem("token");

    const navigate = useNavigate();

    const handleEdit = async () => {
        try
        {
            const response = await axios.post("/api/admin/edit_user", {Id: id, Username: username, Password: password, Admin: admin}, {headers: {Authorization: `Bearer ${token}`}});
        }
        catch
        {

        }
    }
            const [searchParams] = useSearchParams();

    useEffect(() => {
        const data = searchParams.get("data");

        if (!data) return;

        try {
            const decoded = JSON.parse(atob(data));

            setId(decoded.id);
            setUsername(decoded.username);
            setAdmin(decoded.admin);

            console.log(decoded.admin);
        
        } catch (err) {
            console.error("Invalid data:", err);
        }
    }, [searchParams]);

    return (
        <div class="min-h-screen flex justify-center items-center">
            <div class="flex flex-col bg-white/10 w-96 p-2 rounded-lg gap-2">

                <div class="grid grid-cols-3 items-center w-full"> 
                    <div class="justify-self-start ">
                        <IoMdArrowBack class="text-white cursor-pointer" size={18} onClick={() => {navigate("../")}}/>
                    </div>
                
                    <div class="items-center">
                        <span class="text-white text-xl">Edit user</span>
                    </div>
                </div>

                <div class="flex flex-col">
                    <span class="text-white">Username</span>
                    <input class="bg-white/10 p-1 rounded-lg text-white" onChange={(e) => setUsername(e.target.value)} value={username}/>
                </div>

                <div class="flex flex-col">
                    <span class="text-white">New Password</span>
                    <input class="bg-white/10 p-1 rounded-lg text-white" onChange={(e) => setPassword(e.target.value)} value={password} />
                </div>

                <div class="flex flex-row gap-2">
                    <input type="checkbox" onChange={(e) => {setAdmin(e.target.checked)}} checked={admin} />
                    <span class="text-white">Admin</span>
                </div>

                <button class="bg-white/10 p-2 rounded-lg text-white text-semibold hover:[transform:scale(1.01)]" onClick={() => handleEdit()}>Edit User</button>

            </div>
        </div>
    )
}

export default Edit;