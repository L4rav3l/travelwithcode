import React, { useEffect, useState } from "react";
import axios from "axios";
import { MdEdit } from "react-icons/md";
import { FaArrowUp, FaArrowUpLong } from "react-icons/fa6";
import { FaArrowDownLong } from "react-icons/fa6";
import { FaTrashAlt } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import { IoMdAdd } from "react-icons/io";

function Admin()
{
    const [users, setUsers] = useState([]);
    const token = localStorage.getItem("token") || sessionStorage.getItem("token");
    const navigate = useNavigate();

    useEffect(() => {
        const handleList = async () => {
            try
            {
                const response = await axios.get("/api/admin/list", {headers: {Authorization: `Bearer ${token}`}});
                setUsers(response.data);
            }
            catch
            {

            }
        }

        handleList();

    }, [token, axios]);

        const handleList = async () => {
            try
            {
                const response = await axios.get("/api/admin/list", {headers: {Authorization: `Bearer ${token}`}});
                setUsers(response.data);
            }
            catch
            {

            }
        }

    const handlePromote = async (id) => {
        try
        {
            const response = axios.post("/api/admin/add_admin", {Id: id}, {headers: {Authorization: `Bearer ${token}`}});
            handleList();
        }
        catch
        {

        }
    }

    const handleDemote = async (id) => {
        try
        {
            const resposne = axios.post("/api/admin/remove_admin", {Id: id}, {headers: {Authorization: `Bearer ${token}`}});
            handleList();
        }
        catch
        {

        }
    }

    const handleRemove = async (id) => {
        try
        {
            const response = axios.post("/api/admin/delete_user", {Id: id}, {headers: {Authorization: `Bearer ${token}`}});
            handleList();
        }
        catch
        {

        }
    }

    return (
        <div class="h-screen flex flex-col justify-center items-center">
            <div class="flex flex-col bg-white/10 w-96 p-2 rounded-lg gap-2">
            
            <div class="grid grid-cols-3 items-center w-full"> 
                <div class="justify-self-start ">
                    <IoMdAdd class="text-white cursor-pointer" size={18} onClick={() => {navigate("add")}}/>
                </div>

                <div class="items-center">
                    <span class="text-white text-xl">List of Users</span>
                </div>
            </div>

                <div class="flex flex-col justify-center items-center">
                    <table class="block text-white border-separate border-spacing-x-8">
                        <thead>
                            <tr class="">
                                <th class="text-left">Username</th>
                                <th class="text-left">Admin</th>
                                <th class="text-left">Action</th>
                            </tr>
                        </thead>
                        <tbody>
                            {users.map((user, index) => (
                                <tr>
                                    <td class="text-left">{user.username}</td>
                                    <td class="text-center">{user.admin ? "Yes" : "No"}</td>
                                    <td class="text-left gap-1 justify-center">
                                        <div class="flex flex-row justify-center gap-2">
                                            <MdEdit class="cursor-pointer" size={12} onClick={() => {navigate(`edit?id=${user.id}`)}}/>
                                            {user.admin ? (<FaArrowDownLong class="cursor-pointer" size={12} onClick={() => {handleDemote(user.id)}}/>) : (<FaArrowUpLong class="cursor-pointer" size={12} onClick={() => {handlePromote(user.id)}}/>)}
                                            <FaTrashAlt class="cursor-pointer" size={12} onClick={() => {handleRemove(user.id)}}/>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
}

export default Admin;