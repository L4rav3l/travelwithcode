import React, { useEffect, useState } from "react";
import CreateLxc from "./CreateLxc";
import Settings from "./Settings";
import Home from "./Home";
import { useNavigate } from "react-router-dom";
import axios from "axios";  
import Admin from "./Admin";

function Menu()
{

    const [page, setPage] = useState(0);
    const [isAdmin, setIsAdmin] = useState(true);
    const navigate = useNavigate();

    const token = localStorage.getItem("token") || sessionStorage.getItem("token");

    useEffect(() => {
        const handleVerify = async () => {
            try
            {
                const response = await axios.get("/api/auth/verify", {headers: {Authorization:  `Bearer ${token}`}});
            }
            catch
            {
                setPage(3);
            }
        }

        const handleAdmin = async () => {
            try
            {
                const response = await axios.get("/api/admin/check", {headers: {Authorization: `Bearer ${token}`}});
            
                setIsAdmin(response.data);
            }
            catch
            {

            }
        }

        handleVerify();
        handleAdmin();
    }, [])

    if(page === 3)
    {
        localStorage.clear();
        sessionStorage.clear();
        navigate("/");
    }

    return (
        <>
            <div class="min-h-screen flex flex-row">
                <div class="flex flex-col p-2 w-48 h-screen bg-white/10 backdrop-blur gap-8 p-4">
                    <div class="flex flex-col gap-4">
                        <span class="text-center text-white">Travel With Code</span>
                    </div>
                    
                    <div class="flex flex-col gap-2">
                        <div class="flex flex-col p-1 bg-white/10 backdrop-blur hover:[transform:scale(1.03)] rounded-lg cursor-pointer">
                            <span class="text-center text-sm text-white" onClick={() => {setPage(0)}}>Home</span>
                        </div>

                        <div class="flex flex-col p-1 bg-white/10 backdrop-blur hover:[transform:scale(1.03)] rounded-lg cursor-pointer">
                            <span class="text-center text-sm text-white" onClick={() => {setPage(1)}}>Create Container</span>
                        </div>

                        <div class="flex flex-col p-1 bg-white/10 backdrop-blur hover:[transform:scale(1.03)] rounded-lg cursor-pointer">
                            <span class="text-center text-sm text-white" onClick={() => {setPage(2)}}>Settings</span>
                        </div>

                        {isAdmin == true && (<><div class="flex flex-col p-1 bg-white/10 backdrop-blur hover:[transform:scale(1.03)] rounded-lg cursor-pointer">
                            <span class="text-center text-sm text-white" onClick={() => {setPage(4)}}>Admin</span>
                        </div> </> )}

                        <div class="flex flex-col p-1 bg-white/10 backdrop-blur hover:[transform:scale(1.03)] rounded-lg cursor-pointer">
                            <span class="text-center text-sm text-white" onClick={() => {setPage(3)}}>Quit</span>
                        </div>
                    </div>
                </div>

                <div class="flex-1 overflow-auto">

                    {page === 0 && <Home />}
                    {page === 1 && <CreateLxc />}
                    {page === 2 && <Settings />}
                    {page === 4 && <Admin />}

                </div>
            </div>
        </>
    );
}

export default Menu;