using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace _7KAA_ResEdit
{
    struct ResIndex
    {
       char[] name = new char[9];
       int pointer;
    };

    public class ResourceIdx
    {
        
        int DEF_BUF_SIZE = 5120;    // default buffer size : 5K

        ResIndex index_buf;         // index buffer pointer
        char     data_buf;          // data buffer pointer
        uint     data_buf_size;      // size of the data buffer

        bool	 init_flag;
        char     read_all;           // read all data from resource file to memory
        char     use_common_buf;     // use vga's buffer as data buffer or not
        int      rec_count;          // total no. of records
        int      cur_rec_no;         // current record no

        char     user_data_buf;
        uint     user_data_buf_size;
        int		 user_start_read_pos;	// the starting position of the data to be read into the buffer

        ResourceIdx() 	{ init_flag=false; }
        ~ResourceIdx()	{ deinit(); }

        void  init(char resFile, int readAll, int useCommonBuf=0);

        ResourceIdx(char resFile, int readAll, int useCommonBuf=0)
        { 
            init_flag=false; 
            init(resFile, readAll, useCommonBuf); 
        }

        
        void  deinit();

        bool   is_inited() 	{ return init_flag; }

        void	set_user_buf(char* userDataBuf, int bufSize, int userStartReadPos=0);
        void  reset_user_buf();

        char read(char);
        int	read_into_user_buf(char dataName, char userDataBuf, int bufSize, int userStartReadPos=0);

        int   get_index(char);
        char get_data(int);
        char	*data_name(int);

        File get_file(char, int);
        File get_file(int, int);

    }
}
