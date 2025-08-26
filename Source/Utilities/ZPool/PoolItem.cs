using System;

namespace Pool
{
    public interface IItemRecycle
    {
        void OnRecycle();
    }
    // �v�[���I�u�W�F�N�g
    // T�F�I�u�W�F�N�g�̌^�iclass + new�ł���j
    public abstract class PoolItem<T> : IDisposable,IItemRecycle where T : class,new()
    {
        public T Item => _poolObject;
        public bool IsUse => _isUse;

        protected T _poolObject;                                // �I�u�W�F�N�g�̎Q�� 
        //TODO _isUse is not using   
        protected bool _isUse;                                  // �v�[���I�u�W�F�N�g�͎g���Ă��邩�ǂ����i���̃t���O�܂��g���Ă��Ȃ��j
        private Action<PoolItem<T>> _recycleCallback;           // �I�u�W�F�N�g����R�[���o�b�N

        public PoolItem(T obj)
        {
            _poolObject = obj;
            _isUse = false;
            _recycleCallback = null;
        }
        ~PoolItem()
        {
            Dispose(false);
        }

        /// <summary>
        /// �I�u�W�F�N�g���z����
        /// </summary>
        public void OnAllocate()
        {
            _isUse = true;
            SetOnUseItemStatus();
        }
        /// <summary>
        /// ���T�C�N������
        /// </summary>
        public void OnRecycle()
        {
            _isUse = false;
            ResetItemStatus();
            _recycleCallback?.Invoke(this);
        }
        public void Dispose()
        {
            Dispose(true);
        }
        /// <summary>
        /// ���T�C�N���R�[���o�b�N�ݒ�
        /// </summary>
        /// <param name="callback"></param>
        public void SetRecycleCallback(Action<PoolItem<T>> callback)
        {
            _recycleCallback = callback;
        }

        /// <summary>
        /// �v�[���I�u�W�F�N�g�̏�Ԃ�������
        /// </summary>
        protected abstract void ResetItemStatus();

        /// <summary>
        /// �I�u�W�F�N�g�g�p����O�̏����ݒ�
        /// </summary>
        protected abstract void SetOnUseItemStatus();

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                GC.SuppressFinalize(this);
            }
            _recycleCallback = null;
        }

    }

}
