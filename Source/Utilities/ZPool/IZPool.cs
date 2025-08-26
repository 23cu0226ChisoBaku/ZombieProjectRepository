using System;

namespace Pool
{
    public interface IZPool<T> : IDisposable where T : class,new()
    {
        // �v�[����������
        void InitPoolObject(Func<PoolItem<T>> factory);
        // �v�[���I�u�W�F�N�g�𕪔z
        PoolItem<T> Allocate();

        // �T�C�Y�i�g����I�u�W�F�N�g�̐��j
        public int Size{ get;}        
        // �e�ʁi�������I�u�W�F�N�g�̐��̍ő�l  
        public int Capacity{ get;}      
    }
}
