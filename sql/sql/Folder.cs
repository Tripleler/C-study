using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sql
{
    /// <summary>
    /// 폴더
    /// </summary>
    public class Folder
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Field
        ////////////////////////////////////////////////////////////////////////////////////////// Private

        #region Field

        /// <summary>
        /// 디렉토리 정보
        /// </summary>
        private DirectoryInfo directoryInfo;

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Property
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 명칭 - Name

        /// <summary>
        /// 명칭
        /// </summary>
        public string Name
        {
            get
            {
                return this.directoryInfo.Name;
            }
        }

        #endregion
        #region 자식 폴더 리스트 - ChildFolderList

        /// <summary>
        /// 자식 폴더 리스트
        /// </summary>
        public List<Folder> ChildFolderList
        {
            get
            {
                List<Folder> childFolderList = new List<Folder>();

                DirectoryInfo[] childDirectoryInfoArray;

                try
                {
                    childDirectoryInfoArray = this.directoryInfo.GetDirectories();
                }
                catch
                {
                    return childFolderList;
                }

                foreach (DirectoryInfo directoryInfo in childDirectoryInfoArray)
                {
                    childFolderList.Add(new Folder(directoryInfo));
                }

                return childFolderList;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Constructor
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 생성자 - Folder(directoryInfo)

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="directoryInfo">디렉토리 정보</param>
        public Folder(DirectoryInfo directoryInfo)
        {
            this.directoryInfo = directoryInfo;
        }

        #endregion
    }
}
