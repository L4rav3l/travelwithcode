import React, { useState } from "react";

function CreateLxc()
{

    const [haveLXC, setHaveLXC] = useState(true);

    return (
        <>
            <div class="h-screen flex flex-col justify-center items-center">
                <div class="flex flex-col gap-1 bg-white/10 items-center justify-center p-4 rounded-lg">
                    {haveLXC === true && (<><button class="bg-red-600 p-2 rounded-lg text-white">Destroy container</button> <span class="text-white">We will commit the last version of your file!</span> </>)}
                </div>
            </div>
        </>
    );
}

export default CreateLxc;